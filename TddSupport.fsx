#load "./TddSupportTypes.fsx"
#load "./TddSupportHtml.fsx"

open System
open System.Reflection
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open TddSupportTypes

type ReplTest = {
  Name : string
  Test : unit -> unit
}

let runTest test : TestResultInfo =
  let getResult tFunc =
    try
      tFunc ()
      Pass
    with
      | _ as ex -> Fail ex
  { Name  = test.Name ; Result = getResult test.Test }

let runTests = List.map runTest

let testsForExpr (tests: Expr<unit -> unit> list) =
  let rec exprName expr =
    match expr with
    | Lambda (_, innerExpr) -> exprName innerExpr
    | Call (_, mi, _) -> mi.Name
    | _ -> expr.ToString()
  tests
  |> List.map (fun expr ->
    let func = LeafExpressionConverter.EvaluateQuotation expr :?> ( unit -> unit )
    {
      Name = exprName expr
      Test = func
    }
  )

let runTestsExpr = runTests << testsForExpr

let testsForObject (obj:Object) (tests: string list) =
  let typ = obj.GetType()
  let useMethods =
    match tests with
    | [] ->
      typ.GetMethods(System.Reflection.BindingFlags.Instance |||
        System.Reflection.BindingFlags.Public |||
        System.Reflection.BindingFlags.DeclaredOnly)
      |> Array.where (fun mi -> mi.GetParameters().Length = 0)
    | _ ->
      tests
      |> List.map (typ.GetMethod)
      |> List.toArray
  useMethods
  |> Array.toList
  |> List.map (fun mi ->
    {
      Name = mi.Name
      Test = (fun _ -> ignore <| mi.Invoke(obj, [||]))
    }
  )

let runTestsObject o = runTests << testsForObject o