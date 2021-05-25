Imports System

Module Program
    Sub Main(args As String())
        'サーバーに送信するデータを入力してもらう
        Console.WriteLine("文字列を入力し、Enterキーを押してください。")
        Dim sendMsg As String = Console.ReadLine()
        '何も入力されなかった時は終了
        If sendMsg Is Nothing OrElse sendMsg.Length = 0 Then
            Return
        End If

        'サーバーのIPアドレス（または、ホスト名）とポート番号
        Dim ipOrHost As String = "192.168.0.24"
        'Dim ipOrHost As String = "localhost"
        Dim port As Integer = 2001

        'TcpClientを作成し、サーバーと接続する
        Dim tcp As New System.Net.Sockets.TcpClient(ipOrHost, port)
        Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
            DirectCast(tcp.Client.RemoteEndPoint, System.Net.IPEndPoint).Address,
            DirectCast(tcp.Client.RemoteEndPoint, System.Net.IPEndPoint).Port,
            DirectCast(tcp.Client.LocalEndPoint, System.Net.IPEndPoint).Address,
            DirectCast(tcp.Client.LocalEndPoint, System.Net.IPEndPoint).Port)

        'NetworkStreamを取得する
        Dim ns As System.Net.Sockets.NetworkStream = tcp.GetStream()

        '読み取り、書き込みのタイムアウトを10秒にする
        'デフォルトはInfiniteで、タイムアウトしない
        '(.NET Framework 2.0以上が必要)
        ns.ReadTimeout = 10000
        ns.WriteTimeout = 10000

        'サーバーにデータを送信する
        '文字列をByte型配列に変換
        Dim enc As System.Text.Encoding = System.Text.Encoding.UTF8
        Dim sendBytes As Byte() = enc.GetBytes(sendMsg & ControlChars.Lf)
        'データを送信する
        ns.Write(sendBytes, 0, sendBytes.Length)
        Console.WriteLine(sendMsg)

        'サーバーから送られたデータを受信する
        Dim ms As New System.IO.MemoryStream()
        Dim resBytes As Byte() = New Byte(255) {}
        Dim resSize As Integer = 0
        Do
            'データの一部を受信する
            resSize = ns.Read(resBytes, 0, resBytes.Length)
            'Readが0を返した時はサーバーが切断したと判断
            If resSize = 0 Then
                Console.WriteLine("サーバーが切断しました。")
                Exit Do
            End If
            '受信したデータを蓄積する
            ms.Write(resBytes, 0, resSize)
            'まだ読み取れるデータがあるか、データの最後が\nでない時は、
            ' 受信を続ける
        Loop While ns.DataAvailable OrElse
            resBytes(resSize - 1) <> AscW(ControlChars.Lf)
        '受信したデータを文字列に変換
        Dim resMsg As String = enc.GetString(ms.GetBuffer(), 0, CInt(ms.Length))
        ms.Close()
        '末尾の\nを削除
        resMsg = resMsg.TrimEnd(ControlChars.Lf)
        Console.WriteLine(resMsg)

        '閉じる
        ns.Close()
        tcp.Close()
        Console.WriteLine("切断しました。")

        Console.ReadLine()
    End Sub
End Module
