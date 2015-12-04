namespace Helpers

[<AutoOpen>]
module File =
    open System.Text.RegularExpressions

    let RemoveIllegalCharsForFileName (fileName:string) = 
        match fileName with
        | null -> ""
        | _ -> fileName
                .Replace(":","_")
                .Replace("?","_")
                .Replace("/","_")
                .Replace("\"","_")

    
    let GetMatchesFrom (text:string) (pattern:string) = 
        Regex(pattern).Matches(text)
        |> Seq.cast
        |> Seq.map (fun (x:Match) -> x.Groups |> Seq.cast |> Seq.skip 1 |> Seq.map (fun (x:Group) -> x.Value))




[<AutoOpen>]
module WebClient =
    open System.Net
    open System.Threading

    let CreateIe6WebClient() = 
        let wc = new WebClient()
        wc.Headers.["User-Agent"] <-"Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        wc
    
    let DownloadFile (link:string) filePath (timeout:int) = 
        try
            use wc' = CreateIe6WebClient()
            printfn "downloading %s" filePath   
            wc'.DownloadFile(link, filePath)
            printfn "downloaded %s" filePath
            Thread.Sleep(timeout)
            None
        with 
        | ex -> 
            printfn "error downloading %s" filePath 
            Thread.Sleep(timeout)
            Some(link)
