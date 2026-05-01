Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Service1
    Inherits System.Web.Services.WebService
    Private strPathSchemaGet_Ticket As String = My.Settings.SchemaGet_Ticket
    Private strPathSchemaCheck_Ticket As String = My.Settings.SchemaCheck_Ticket
    Private strPathSchemaUpdate_Ticket As String = My.Settings.SchemaUpdate_Ticket

    <WebMethod()>
    Public Function Get_Ticket(ByVal ds As DataSet) As DataSet
        ''ByVal ds As DataSet
        'Dim MyTableH As New DataTable
        'Dim MyColumn As New DataColumn

        'MyTableH.TableName = "InputData"
        'MyColumn = New DataColumn("User", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Page_Name", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Parameter", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)

        'Dim dataRowH As DataRow
        'dataRowH = MyTableH.NewRow()
        'dataRowH("User") = "chairungreang.b"
        'dataRowH("Page_Name") = "Central Payment"
        'dataRowH("Parameter") = "AG00123|AG00222|120000"
        'MyTableH.Rows.Add(dataRowH)

        'Dim ds As New DataSet
        'ds.Tables.Add(MyTableH)


        Dim myconn = New SqlConnection(My.Settings.UtilityConnectionString)
        Dim cmd As SqlCommand
        Dim mysql As String
        Dim dr As DataRow
        Dim ticket_no As String = Now().ToString("HHyyyymmMMssddfff")
        Dim dataSet As New DataSet
        dataSet.ReadXmlSchema(Server.MapPath(strPathSchemaGet_Ticket))
        Dim dataRow As DataRow
        dataRow = dataSet.Tables("WS_RETURN").NewRow
        'dataRow("WS_RETURE_CODE") = "-1"
        'dataRow("WS_RETURE_DESCRIPYION") = "Unknow"
        dataRow("WS_RETURN_CODE") = "-1"
        dataRow("WS_RETURN_DESCRIPTION") = "Unknow"
        dataRow("User") = ds.Tables("InputData").Rows(0).Item("User")
        dataRow("Page_Name") = ds.Tables("InputData").Rows(0).Item("Page_Name")
        dataRow("Parameter") = ds.Tables("InputData").Rows(0).Item("Parameter")
        If ds.Tables("InputData").Rows(0).Item("User").ToString.Trim.Length = 0 Or
        ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim.Length = 0 Or
        ds.Tables("InputData").Rows(0).Item("Parameter").ToString.Trim.Length = 0 Then
            dataRow("WS_RETURN_DESCRIPTION") = "ข้อมูลไม่ถูกต้องกรุณาทำรายการใหม่"
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        Else
            mysql = "INSERT INTO dbo.tb_ticket_info " &
                    "(Ticket_Code, Page_Name, User_Code, Parameter, update_flag) " &
                    "VALUES (@Ticket_Code, @Page_Name, @User_Code, @Parameter, 'N')"
            Try
                myconn.Open()
                cmd = New SqlCommand(mysql, myconn)

                cmd.Parameters.Add("@Ticket_Code", SqlDbType.NVarChar, 50)
                cmd.Parameters("@Ticket_Code").Value = ticket_no
                cmd.Parameters.Add("@Page_Name", SqlDbType.NVarChar, 100)
                cmd.Parameters("@Page_Name").Value = ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim
                cmd.Parameters.Add("@User_Code", SqlDbType.NVarChar, 50)
                cmd.Parameters("@User_Code").Value = ds.Tables("InputData").Rows(0).Item("User").ToString.Trim
                cmd.Parameters.Add("@Parameter", SqlDbType.NVarChar, 250)
                cmd.Parameters("@Parameter").Value = ds.Tables("InputData").Rows(0).Item("Parameter").ToString.Trim
                cmd.ExecuteNonQuery()
                myconn.Close()

                dr = dataSet.Tables("WS_INFORMATION").NewRow
                dr("Ticket_Code") = ticket_no
                dataSet.Tables("WS_INFORMATION").Rows.Add(dr)
                dataRow("WS_RETURN_CODE") = "1"
                dataRow("WS_RETURN_DESCRIPTION") = "OK"

            Catch ex As Exception
                dataRow("WS_RETURN_DESCRIPTION") = "ผิดพลาดเกี่ยวกับฐานข้อมูล: " & ex.Message
            End Try
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        End If

        Return dataSet
    End Function
    <WebMethod()> _
    Public Function Check_Ticket(ByVal ds As DataSet) As DataSet
        'ByVal ds As DataSet
        'Dim MyTableH As New DataTable
        'Dim MyColumn As New DataColumn

        'MyTableH.TableName = "InputData"
        'MyColumn = New DataColumn("Ticket_Code", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Page_Name", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("update_flag", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)

        'Dim dataRowH As DataRow
        'dataRowH = MyTableH.NewRow()
        'dataRowH("Ticket_Code") = "18201500073820581"
        'dataRowH("Page_Name") = "Central Payment"
        'dataRowH("update_flag") = "Y"
        'MyTableH.Rows.Add(dataRowH)

        'Dim ds As New DataSet
        'ds.Tables.Add(MyTableH)

        Dim myconn = New SqlConnection(My.Settings.UtilityConnectionString)
        Dim cmd As SqlCommand
        Dim mysql As String
        Dim t_update_flag As String = ""
        Dim dr As DataRow
        Dim dr_ticket As DataRow
        Dim dataSet As New DataSet
        dataSet.ReadXmlSchema(Server.MapPath(strPathSchemaCheck_Ticket))
        Dim dataRow As DataRow
        dataRow = dataSet.Tables("WS_RETURN").NewRow
        dataRow("WS_RETURN_CODE") = "-1"
        dataRow("WS_RETURN_DESCRIPTION") = "Unknow"
        dataRow("Ticket_Code") = ds.Tables("InputData").Rows(0).Item("Ticket_Code")
        dataRow("Page_Name") = ds.Tables("InputData").Rows(0).Item("Page_Name")
        Try
            t_update_flag = ds.Tables("InputData").Rows(0).Item("update_flag")
            dataRow("update_flag") = t_update_flag
        Catch ex As Exception
            dataRow("update_flag") = ""
            t_update_flag = "N"
        End Try
        If ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Length = 0 Or _
        ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Length = 0 Then
            dataRow("WS_RETURN_DESCRIPTION") = "ข้อมูลไม่ถูกต้องกรุณาทำรายการใหม่"
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        Else
            Try
                mysql = "SELECT User_Code, Parameter from dbo.tb_ticket_info " & _
                        "where (Ticket_Code = @Ticket_Code) " & _
                        "and (Page_Name = @Page_Name) "
                If t_update_flag = "Y" Then
                    mysql &= "and (isnull(update_flag, 'N') = 'Y') "
                End If
                Dim dt As New DataTable
                myconn.Open()
                cmd = New SqlCommand(mysql, myconn)
                cmd.Parameters.Add("@Ticket_Code", SqlDbType.NVarChar, 50)
                cmd.Parameters("@Ticket_Code").Value = ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Trim
                cmd.Parameters.Add("@Page_Name", SqlDbType.NVarChar, 100)
                cmd.Parameters("@Page_Name").Value = ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim
                Dim da = New SqlDataAdapter(cmd)
                da.Fill(dt)
                myconn.Close()
                If dt.Rows.Count > 0 Then
                    For Each dr_ticket In dt.Rows
                        dr = dataSet.Tables("WS_INFORMATION").NewRow
                        dr("user") = dr_ticket.Item("User_Code").ToString.Trim
                        dr("Parameter") = dr_ticket.Item("Parameter").ToString.Trim
                        dataSet.Tables("WS_INFORMATION").Rows.Add(dr)
                    Next
                    mysql = "DELETE FROM dbo.tb_ticket_info " & _
                            "where (Ticket_Code = @Ticket_Code) " & _
                            "and (Page_Name = @Page_Name)"
                    myconn.Open()
                    cmd = New SqlCommand(mysql, myconn)

                    cmd.Parameters.Add("@Ticket_Code", SqlDbType.NVarChar, 50)
                    cmd.Parameters("@Ticket_Code").Value = ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Trim
                    cmd.Parameters.Add("@Page_Name", SqlDbType.NVarChar, 100)
                    cmd.Parameters("@Page_Name").Value = ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim
                    cmd.ExecuteNonQuery()
                    myconn.Close()
                    dataRow("WS_RETURN_CODE") = "1"
                    dataRow("WS_RETURN_DESCRIPTION") = "OK"
                Else
                    dataRow("WS_RETURN_CODE") = "0"
                    dataRow("WS_RETURN_DESCRIPTION") = "No data found"
                End If
            Catch ex As Exception
                dataRow("WS_RETURN_DESCRIPTION") = "ผิดพลาดเกี่ยวกับฐานข้อมูล: " & ex.Message
            End Try
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        End If

        Return dataSet
    End Function
    <WebMethod()> _
        Public Function Update_Ticket(ByVal ds As DataSet) As DataSet
        'ByVal ds As DataSet
        'Dim MyTableH As New DataTable
        'Dim MyColumn As New DataColumn

        'MyTableH.TableName = "InputData"
        'MyColumn = New DataColumn("User", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Page_Name", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Ticket_Code", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)
        'MyColumn = New DataColumn("Parameter", System.Type.GetType("System.String"))
        'MyTableH.Columns.Add(MyColumn)

        'Dim dataRowH As DataRow
        'dataRowH = MyTableH.NewRow()
        'dataRowH("User") = "chairungreang.b"
        'dataRowH("Page_Name") = "Central Payment"
        'dataRowH("Ticket_Code") = "18201500073820581"
        'dataRowH("Parameter") = "T11255222"
        'MyTableH.Rows.Add(dataRowH)

        'Dim ds As New DataSet
        'ds.Tables.Add(MyTableH)


        Dim myconn = New SqlConnection(My.Settings.UtilityConnectionString)
        Dim cmd As SqlCommand
        Dim mysql As String
        Dim dr As DataRow
        'Dim ticket_no As String = Now().ToString("HHyyyymmMMssddfff")
        Dim dataSet As New DataSet
        dataSet.ReadXmlSchema(Server.MapPath(strPathSchemaUpdate_Ticket))
        Dim dataRow As DataRow
        dataRow = dataSet.Tables("WS_RETURN").NewRow
        'dataRow("WS_RETURE_CODE") = "-1"
        'dataRow("WS_RETURE_DESCRIPYION") = "Unknow"
        dataRow("WS_RETURN_CODE") = "-1"
        dataRow("WS_RETURN_DESCRIPTION") = "Unknow"
        dataRow("User") = ds.Tables("InputData").Rows(0).Item("User")
        dataRow("Page_Name") = ds.Tables("InputData").Rows(0).Item("Page_Name")
        dataRow("Ticket_Code") = ds.Tables("InputData").Rows(0).Item("Ticket_Code")
        dataRow("Parameter") = ds.Tables("InputData").Rows(0).Item("Parameter")
        If ds.Tables("InputData").Rows(0).Item("User").ToString.Trim.Length = 0 Or _
        ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim.Length = 0 Or _
        ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Trim.Length = 0 Or _
        ds.Tables("InputData").Rows(0).Item("Parameter").ToString.Trim.Length = 0 Then
            dataRow("WS_RETURN_DESCRIPTION") = "ข้อมูลไม่ถูกต้องกรุณาทำรายการใหม่"
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        Else
            mysql = "DELETE FROM dbo.tb_ticket_info " & _
                    "where (Ticket_Code = @Ticket_Code) " & _
                    "and (Page_Name = @Page_Name)"
            myconn.Open()
            cmd = New SqlCommand(mysql, myconn)

            cmd.Parameters.Add("@Ticket_Code", SqlDbType.NVarChar, 50)
            cmd.Parameters("@Ticket_Code").Value = ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Trim
            cmd.Parameters.Add("@Page_Name", SqlDbType.NVarChar, 100)
            cmd.Parameters("@Page_Name").Value = ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim
            cmd.ExecuteNonQuery()
            myconn.Close()

            mysql = "INSERT INTO dbo.tb_ticket_info " & _
                    "(Ticket_Code, Page_Name, User_Code, Parameter, update_flag) " & _
                    "VALUES (@Ticket_Code, @Page_Name, @User_Code, @Parameter, 'Y')"
            Try
                myconn.Open()
                cmd = New SqlCommand(mysql, myconn)

                cmd.Parameters.Add("@Ticket_Code", SqlDbType.NVarChar, 50)
                cmd.Parameters("@Ticket_Code").Value = ds.Tables("InputData").Rows(0).Item("Ticket_Code").ToString.Trim
                cmd.Parameters.Add("@Page_Name", SqlDbType.NVarChar, 100)
                cmd.Parameters("@Page_Name").Value = ds.Tables("InputData").Rows(0).Item("Page_Name").ToString.Trim
                cmd.Parameters.Add("@User_Code", SqlDbType.NVarChar, 50)
                cmd.Parameters("@User_Code").Value = ds.Tables("InputData").Rows(0).Item("User").ToString.Trim
                cmd.Parameters.Add("@Parameter", SqlDbType.NVarChar, 250)
                cmd.Parameters("@Parameter").Value = ds.Tables("InputData").Rows(0).Item("Parameter").ToString.Trim
                cmd.ExecuteNonQuery()
                myconn.Close()

                dataRow("WS_RETURN_CODE") = "1"
                dataRow("WS_RETURN_DESCRIPTION") = "OK"

            Catch ex As Exception
                dataRow("WS_RETURN_DESCRIPTION") = "ผิดพลาดเกี่ยวกับฐานข้อมูล: " & ex.Message
            End Try
            dataSet.Tables("WS_RETURN").Rows.Add(dataRow)
        End If

        Return dataSet
    End Function
    '<WebMethod()> _
    'Public Function LMG_encryption(ByVal inputText As String, ByVal encryptionkey As String) As String
    '    'Dim encryptionkey As String = "LMGT193BX628TD57"
    '    Dim keybytes As Byte() = Encoding.ASCII.GetBytes(encryptionkey.Length.ToString())
    '    Dim rijndaelCipher As New RijndaelManaged()
    '    Dim plainText As Byte() = Encoding.Unicode.GetBytes(inputText)
    '    Dim pwdbytes As New PasswordDeriveBytes(encryptionkey, keybytes)
    '    Using encryptrans As ICryptoTransform = rijndaelCipher.CreateEncryptor(pwdbytes.GetBytes(32), pwdbytes.GetBytes(16))
    '        Using mstrm As New MemoryStream()
    '            Using cryptstm As New CryptoStream(mstrm, encryptrans, CryptoStreamMode.Write)
    '                cryptstm.Write(plainText, 0, plainText.Length)
    '                cryptstm.Close()
    '                Return Convert.ToBase64String(mstrm.ToArray())
    '            End Using
    '        End Using
    '    End Using
    'End Function
    '<WebMethod()>
    'Public Function LMG_Decryption(ByVal encryptText As String, ByVal encryptionkey As String) As String
    '    'Dim encryptionkey As String = "LMGT193BX628TD57"
    '    Dim keybytes As Byte() = Encoding.ASCII.GetBytes(encryptionkey.Length.ToString())
    '    Dim rijndaelCipher As New RijndaelManaged()
    '    Dim encryptedData As Byte() = Convert.FromBase64String(encryptText.Replace(" ", "+"))
    '    Dim pwdbytes As New PasswordDeriveBytes(encryptionkey, keybytes)
    '    Using decryptrans As ICryptoTransform = rijndaelCipher.CreateDecryptor(pwdbytes.GetBytes(32), pwdbytes.GetBytes(16))
    '        Using mstrm As New MemoryStream(encryptedData)
    '            Using cryptstm As New CryptoStream(mstrm, decryptrans, CryptoStreamMode.Read)
    '                Dim plainText As Byte() = New Byte(encryptedData.Length - 1) {}
    '                Dim decryptedCount As Integer = cryptstm.Read(plainText, 0, plainText.Length)
    '                Return Encoding.Unicode.GetString(plainText, 0, decryptedCount)
    '            End Using
    '        End Using
    '    End Using
    'End Function
    <WebMethod()>
    Public Function LMG_Encrypt(ByVal key As String, ByVal data As String) As String
        Dim encData As String = Nothing
        Dim keys As Byte()() = GetHashKeys(key)

        Try
            encData = EncryptStringToBytes_Aes(data, keys(0), keys(1))
        Catch __unusedCryptographicException1__ As CryptographicException
        Catch __unusedArgumentNullException2__ As ArgumentNullException
        End Try

        Return encData
    End Function
    <WebMethod()>
    Public Function LMG_Decrypt(ByVal key As String, ByVal data As String) As String
        Dim decData As String = Nothing
        Dim keys As Byte()() = GetHashKeys(key)

        Try
            decData = DecryptStringFromBytes_Aes(data, keys(0), keys(1))
        Catch __unusedCryptographicException1__ As CryptographicException
        Catch __unusedArgumentNullException2__ As ArgumentNullException
        End Try

        Return decData
    End Function
    Private Function GetHashKeys(ByVal key As String) As Byte()()
        Dim result As Byte()() = New Byte(1)() {}
        Dim enc As Encoding = Encoding.UTF8
        Dim sha2 As SHA256 = New SHA256CryptoServiceProvider()
        Dim rawKey As Byte() = enc.GetBytes(key)
        Dim rawIV As Byte() = enc.GetBytes(key)
        Dim hashKey As Byte() = sha2.ComputeHash(rawKey)
        Dim hashIV As Byte() = sha2.ComputeHash(rawIV)
        Array.Resize(hashIV, 16)
        result(0) = hashKey
        result(1) = hashIV
        Return result
    End Function
    Private Shared Function EncryptStringToBytes_Aes(ByVal plainText As String, ByVal Key As Byte(), ByVal IV As Byte()) As String
        If plainText Is Nothing OrElse plainText.Length <= 0 Then Throw New ArgumentNullException("plainText")
        If Key Is Nothing OrElse Key.Length <= 16 Then Throw New ArgumentNullException("Key")
        If IV Is Nothing OrElse IV.Length <= 0 Then Throw New ArgumentNullException("IV")
        Dim encrypted As Byte()

        Using aesAlg As AesManaged = New AesManaged()
            aesAlg.Key = Key
            aesAlg.IV = IV
            Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)

            Using msEncrypt As MemoryStream = New MemoryStream()

                Using csEncrypt As CryptoStream = New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)

                    Using swEncrypt As StreamWriter = New StreamWriter(csEncrypt)
                        swEncrypt.Write(plainText)
                    End Using

                    encrypted = msEncrypt.ToArray()
                End Using
            End Using
        End Using

        Return Convert.ToBase64String(encrypted)
    End Function
    Private Shared Function DecryptStringFromBytes_Aes(ByVal cipherTextString As String, ByVal Key As Byte(), ByVal IV As Byte()) As String
        Dim cipherText As Byte() = Convert.FromBase64String(cipherTextString)
        If cipherText Is Nothing OrElse cipherText.Length <= 0 Then Throw New ArgumentNullException("cipherText")
        If Key Is Nothing OrElse Key.Length <= 0 Then Throw New ArgumentNullException("Key")
        If IV Is Nothing OrElse IV.Length <= 0 Then Throw New ArgumentNullException("IV")
        Dim plaintext As String = Nothing

        Using aesAlg As Aes = Aes.Create()
            aesAlg.Key = Key
            aesAlg.IV = IV
            Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)

            Using msDecrypt As MemoryStream = New MemoryStream(cipherText)

                Using csDecrypt As CryptoStream = New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

                    Using srDecrypt As StreamReader = New StreamReader(csDecrypt)
                        plaintext = srDecrypt.ReadToEnd()
                    End Using
                End Using
            End Using
        End Using

        Return plaintext
    End Function
End Class
