Private Function add_cert(ByVal FileNM As String,
                          ByVal outputFile As String,
                          ByVal Print_Cert As String,
                          ByVal RegKey As String) As String

    Dim res As String = ""
    Dim objPDF As PdfManager = Nothing
    Dim objDoc As IPdfDocument = Nothing
    Dim objCer As ICryptoManager = Nothing
    Dim Context As Object = Nothing
    Dim Msg As Object = Nothing
    Dim Store As Object = Nothing
    Dim Cert As Object = Nothing

    Try
        '==============================
        ' ENTER
        '==============================
        ErrorLog("[ADD_CERT] ENTER")
        ErrorLog("[ADD_CERT] FileNM = " & FileNM)
        ErrorLog("[ADD_CERT] OutputFile = " & outputFile)
        ErrorLog("[ADD_CERT] Print_Cert = " & Print_Cert)
        ErrorLog("[ADD_CERT] RegKey length = " & RegKey.Length)

        If String.IsNullOrEmpty(Print_Cert) Then
            Throw New Exception("Print_Cert is empty")
        End If

        If RegKey.Length = 0 Then
            Throw New Exception("RegKey is empty")
        End If

        '==============================
        ' PDF MANAGER
        '==============================
        objPDF = New PdfManager
        objPDF.RegKey = RegKey
        ErrorLog("[ADD_CERT] PdfManager created")

        objDoc = objPDF.OpenDocument(FileNM)
        If objDoc Is Nothing Then
            Throw New Exception("OpenDocument failed")
        End If
        ErrorLog("[ADD_CERT] PDF document opened")

        '==============================
        ' CRYPTO MANAGER
        '==============================
        objCer = New CryptoManager
        ErrorLog("[ADD_CERT] CryptoManager created")

        ErrorLog("[ADD_CERT] Before OpenContext")
        Context = objCer.OpenContext("", True, Nothing)
        If Context Is Nothing Then
            Throw New Exception("OpenContext failed")
        End If
        ErrorLog("[ADD_CERT] Context opened")

        Msg = Context.CreateMessage
        If Msg Is Nothing Then
            Throw New Exception("CreateMessage failed")
        End If
        ErrorLog("[ADD_CERT] Message created")

        objCer.RevertToSelf()
        ErrorLog("[ADD_CERT] RevertToSelf OK")

        '==============================
        ' LOAD CERTIFICATE
        '==============================
        Dim arr() As String = Print_Cert.Split("|"c)
        If arr.Length < 2 Then
            Throw New Exception("Print_Cert format invalid (path|password)")
        End If

        Dim pfxPath As String = arr(0)
        Dim pfxPwd As String = arr(1)

        ErrorLog("[ADD_CERT] PFX Path = " & pfxPath)
        ErrorLog("[ADD_CERT] PFX Exists = " & File.Exists(pfxPath))
        ErrorLog("[ADD_CERT] PFX Password Length = " & pfxPwd.Length)

        If Not File.Exists(pfxPath) Then
            Throw New Exception("PFX file not found")
        End If

        Store = objCer.OpenStorefromPFX(pfxPath, pfxPwd)
        If Store Is Nothing Then
            Throw New Exception("OpenStorefromPFX failed")
        End If
        ErrorLog("[ADD_CERT] Store opened")

        ErrorLog("[ADD_CERT] Cert Count = " & Store.Certificates.Count)
        If Store.Certificates.Count = 0 Then
            Throw New Exception("No certificate in PFX")
        End If

        Cert = Store.Certificates.Item(1)
        If Cert Is Nothing Then
            Throw New Exception("Certificate item is Nothing")
        End If
        ErrorLog("[ADD_CERT] Certificate loaded")

        Msg.SetSignerCert(Cert)
        ErrorLog("[ADD_CERT] SetSignerCert OK")

        '==============================
        ' SIGN PDF
        '==============================
        ErrorLog("[ADD_CERT] Before Sign")
        objDoc.Sign(Msg, "ePolicy")
        ErrorLog("[ADD_CERT] Sign completed")

        objDoc.Save(outputFile)
        ErrorLog("[ADD_CERT] PDF saved")

        objDoc.Close()
        ErrorLog("[ADD_CERT] Document closed")

        res = "OK"
        ErrorLog("[ADD_CERT] END OK")

    Catch ex As Exception
        ErrorLog("[ADD_CERT][ERROR] " & ex.Message)
        ErrorLog("[ADD_CERT][STACK] " & ex.StackTrace)
        res = "ERROR : " & ex.Message
    End Try

    Return res
End Function
