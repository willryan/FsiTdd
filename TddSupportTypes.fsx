open System

[<StructuredFormatDisplay("{Format}")>]
type TestResult =
  | Pass
  | Fail of Exception
  member x.Format =
    match x with
      | Pass -> "_____PASS_____"
      | Fail ex -> sprintf "!!!!!FAIL!!!!\n%A" ex

[<StructuredFormatDisplay("{Format}")>]
type TestResultInfo =
  {
    Name : string
    Result : TestResult
  }
  member x.Format =
    sprintf "<<<<<TEST>>>> %s\n%s" x.Name (x.Result.Format)
