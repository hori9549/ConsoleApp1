Imports System

Module Program
    Sub Main(args As String())
        '�T�[�o�[�ɑ��M����f�[�^����͂��Ă��炤
        Console.WriteLine("���������͂��AEnter�L�[�������Ă��������B")
        Dim sendMsg As String = Console.ReadLine()
        '�������͂���Ȃ��������͏I��
        If sendMsg Is Nothing OrElse sendMsg.Length = 0 Then
            Return
        End If

        '�T�[�o�[��IP�A�h���X�i�܂��́A�z�X�g���j�ƃ|�[�g�ԍ�
        Dim ipOrHost As String = "192.168.0.24"
        'Dim ipOrHost As String = "localhost"
        Dim port As Integer = 2001

        'TcpClient���쐬���A�T�[�o�[�Ɛڑ�����
        Dim tcp As New System.Net.Sockets.TcpClient(ipOrHost, port)
        Console.WriteLine("�T�[�o�[({0}:{1})�Ɛڑ����܂���({2}:{3})�B",
            DirectCast(tcp.Client.RemoteEndPoint, System.Net.IPEndPoint).Address,
            DirectCast(tcp.Client.RemoteEndPoint, System.Net.IPEndPoint).Port,
            DirectCast(tcp.Client.LocalEndPoint, System.Net.IPEndPoint).Address,
            DirectCast(tcp.Client.LocalEndPoint, System.Net.IPEndPoint).Port)

        'NetworkStream���擾����
        Dim ns As System.Net.Sockets.NetworkStream = tcp.GetStream()

        '�ǂݎ��A�������݂̃^�C���A�E�g��10�b�ɂ���
        '�f�t�H���g��Infinite�ŁA�^�C���A�E�g���Ȃ�
        '(.NET Framework 2.0�ȏオ�K�v)
        ns.ReadTimeout = 10000
        ns.WriteTimeout = 10000

        '�T�[�o�[�Ƀf�[�^�𑗐M����
        '�������Byte�^�z��ɕϊ�
        Dim enc As System.Text.Encoding = System.Text.Encoding.UTF8
        Dim sendBytes As Byte() = enc.GetBytes(sendMsg & ControlChars.Lf)
        '�f�[�^�𑗐M����
        ns.Write(sendBytes, 0, sendBytes.Length)
        Console.WriteLine(sendMsg)

        '�T�[�o�[���瑗��ꂽ�f�[�^����M����
        Dim ms As New System.IO.MemoryStream()
        Dim resBytes As Byte() = New Byte(255) {}
        Dim resSize As Integer = 0
        Do
            '�f�[�^�̈ꕔ����M����
            resSize = ns.Read(resBytes, 0, resBytes.Length)
            'Read��0��Ԃ������̓T�[�o�[���ؒf�����Ɣ��f
            If resSize = 0 Then
                Console.WriteLine("�T�[�o�[���ؒf���܂����B")
                Exit Do
            End If
            '��M�����f�[�^��~�ς���
            ms.Write(resBytes, 0, resSize)
            '�܂��ǂݎ���f�[�^�����邩�A�f�[�^�̍Ōオ\n�łȂ����́A
            ' ��M�𑱂���
        Loop While ns.DataAvailable OrElse
            resBytes(resSize - 1) <> AscW(ControlChars.Lf)
        '��M�����f�[�^�𕶎���ɕϊ�
        Dim resMsg As String = enc.GetString(ms.GetBuffer(), 0, CInt(ms.Length))
        ms.Close()
        '������\n���폜
        resMsg = resMsg.TrimEnd(ControlChars.Lf)
        Console.WriteLine(resMsg)

        '����
        ns.Close()
        tcp.Close()
        Console.WriteLine("�ؒf���܂����B")

        Console.ReadLine()
    End Sub
End Module
