#load "./TddSupportTypes.fsx"

open System
open TddSupportTypes

let rec exceptionToHtml (ex:Exception) : string =
  if not (isNull ex.InnerException) then
    exceptionToHtml ex.InnerException
  else
    let messageLines = ex.Message.Split('\n')
    let messageHtmlLine = sprintf "<div>%s</div>"
    let stackTraceLines = ex.StackTrace.Split('\n')
    let stackTraceHtmlLine = sprintf "<div class='test-stack-trace-content-line'>%s</div>"
    let lines =
      seq {
        yield "<div class='test-error-message'>"
        yield! Array.map messageHtmlLine messageLines
        yield "</div>"
        yield "<div class='test-stack-trace'>"
        yield "  <div class='test-stack-trace-label'>Stack trace:</div>"
        yield "  <div class='test-stack-trace-content'>"
        yield! Array.map stackTraceHtmlLine stackTraceLines
        yield "  </div>"
        yield "</div>"
      }
    String.Join("", lines)

let resultHtml = function
  | Pass -> "<span class='success'>✔</span>"
  | Fail ex -> "<span class='failure'>✘</span>"

let detailHtml = function
  | Pass -> ""
  | Fail ex -> exceptionToHtml ex

let htmlWrapper body =
  let str = """<html>
       <header>
          <style type='text/css'>
            body { font-family: Calibri, Verdana, Arial, sans-serif; background-color: White; color: Black; }
            h2,h3,h4,h5 { margin: 0; padding: 0; }
            h3 { font-weight: normal; }
            h4 { margin: 0.5em 0; }
            h5 { font-weight: normal; font-style: italic; margin-bottom: 0.75em; }
            h6 { font-size: 0.9em; font-weight: bold; margin: 0.5em 0 0 0.75em; padding: 0; }
            pre,table { font-family: Consolas; font-size: 0.8em; margin: 0 0 0 1em; padding: 0; }
            table { padding-bottom: 0.25em; }
            th { padding: 0 0.5em; border-right: 1px solid #bbb; text-align: left; }
            td { padding-left: 0.5em; }
            .divided { border-top: solid 1px #f0f5fa; padding-top: 0.5em; }
            .row, .altrow { padding: 0.1em 0.3em; }
            .row { background-color: #f0f5fa; }
            .altrow { background-color: #e1ebf4; }
            .success, .failure, .skipped { font-family: Arial Unicode MS; font-weight: normal; float: left; width: 1em; display: block; }
            .success { color: #0c0; }
            .failure { color: #c00; }
            .test-name { font-weight: bold; }
            .test-error-message { font-style: italic; color: red; margin-left: 16px; }
            .test-stack-trace { margin-left: 16px; }
            .test-stack-trace-content { font-size: 66%; margin-left: 20px; }
          </style>
        </header>
        <body>"""

  let str2 = """
        </body>
      </html>
  """
  str + body + str2


let testHtml test =
  let summary = resultHtml test.Result
  let detail = detailHtml test.Result
  sprintf "<div class='row'>
    %s<span class='test-name'>&nbsp;%s</span>
    %s
  </div>" summary test.Name detail
  |> htmlWrapper

let printer (h:TestResultInfo list) : seq<string*string> * string =
  let strings = h |> List.map testHtml
  (Seq.empty,String.Join("", strings))


#if HAS_FSI_ADDHTMLPRINTER
fsi.AddHtmlPrinter(printer)
#endif
