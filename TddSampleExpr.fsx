fsi.ShowDeclarationValues <- false
fsi.ShowProperties <- false
fsi.ShowIEnumerable <- false

#load "TddSupport.fsx"
#r "./packages/xunit.extensibility.core.2.1.0/lib/dotnet/xunit.core.dll"
#r "./packages/xunit.assert.2.1.0/lib/dotnet/xunit.assert.dll"
#r "./packages/xunit.abstractions.2.0.0/lib/net35/xunit.abstractions.dll"

#I "./packages/FsUnit.xUnit.1.4.1.0/lib/net45"
#r "FsUnit.Xunit.dll"
#r "NHamcrest.dll"

open FsUnit.Xunit

module MyNewStuff =
  let add x y =
    x + y
  let subtract x y =
    x - y + 1
  let divide x y =
    x / y

//[<Fact>]
let ``test adding``() =
  MyNewStuff.add 2 3 |> should equal 5
  MyNewStuff.add 2 2 |> should equal 4

let ``test subtraction``() =
  MyNewStuff.subtract 3 1 |> should equal 2

//[<Fact>]
let ``test division``() =
  MyNewStuff.divide 2 2 |> should equal 1
  MyNewStuff.divide 32 8 |> should equal 4
  MyNewStuff.divide 9 0 |> should equal 1

TddSupport.runTestsExpr [
  <@ ``test adding`` @>
  <@ ``test subtraction`` @>
  <@ ``test division`` @>
]
