// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open Helpers
open System.Text
open System.IO

let getPodcastNumber (title:string) = 
    title.Trim().Substring("Episode ".Length)
    |> fun x -> x.Substring(0, x.IndexOf(" ")).Trim()

[<EntryPoint>]
let main argv = 
    let pageNumbers = [1 .. 11]
    let folder = "azpodcasts\\"
    let failedFiles = 
        pageNumbers 
        |> Seq.map (fun x -> "http://azpodcast.azurewebsites.net/category/Podcast?page=" + x.ToString())
        |> Seq.map GetStringFromUrl    
        |> Seq.map (fun x -> System.Threading.Thread.Sleep(5000); x)
        |> Seq.map (fun html -> GetSingleMatchesFrom html """class="taggedlink">(.*?)</a>""" )
        |> Seq.map (fun rawTitles -> Seq.map RemoveIllegalCharsForFileName rawTitles)
        |> Seq.concat
        |> Seq.map (fun title -> (getPodcastNumber title,title))
        |> Seq.map (fun (nr, title) -> 
                        DownloadFile 
                            ("http://azpodcast.blob.core.windows.net/episodes/Episode"+nr+".mp3")
                            (folder + title + ".mp3")
                            10000
                    )
        |> Seq.filter (fun x -> x.IsSome)
        |> Seq.map (fun x -> x.Value)
        |> Seq.fold (fun (acc:StringBuilder) failedLink -> acc.AppendLine(failedLink)) (StringBuilder())
        |> fun log -> File.WriteAllText("log.txt", log.ToString())
       
   

    printfn "%A" argv
    0 // return an integer exit code
