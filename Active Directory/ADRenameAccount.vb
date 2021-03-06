Imports Ayehu.Sdk.ActivityCreation.Interfaces
Imports Ayehu.Sdk.ActivityCreation.Extension
Imports System.Text
Imports System
Imports System.Xml
Imports System.Data
Imports System.IO
Imports System.Management
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.DirectoryServices
Imports System.Net

Namespace Ayehu.Sdk.ActivityCreation
    Public Class ActivityClass
        Implements IActivity


        Private Const DefaultAdPort As String = "389"
        Public HostName As String
        Public UserName As String
        Public Password As String
        Public ADUserName As String
        Public ADNewName As String
        Public AccountType As String
        Public SecurePort As String

        Public Function Execute() As ICustomActivityResult Implements IActivity.Execute

            Dim dt As DataTable = New DataTable("resultSet")
            dt.Columns.Add("Result", GetType(String))

            If String.IsNullOrEmpty(SecurePort) = True Then
                SecurePort = DefaultAdPort
            End If

            If IsNumeric(SecurePort) = False Then
                Dim msg As String = "Port parameter must be number"
                Throw New ApplicationException(msg)
            End If

            Dim de As DirectoryEntry = GetAdEntry(HostName, SecurePort, UserName, Password)

            Dim ds As DirectorySearcher = New DirectorySearcher(de)
            Select Case LCase(AccountType)
                Case "user"
                    ds.Filter = "(&(objectClass=user) (sAMAccountname=" + ADUserName + "))"
                Case "computer"
                    ds.Filter = "(&(objectClass=computer) (sAMAccountname=" + ADUserName + "$))"
            End Select
            ds.SearchScope = SearchScope.Subtree
            Dim results As SearchResult = ds.FindOne()
            If results IsNot Nothing Then
                Dim deuser As DirectoryEntry = results.GetDirectoryEntry()
                deuser.Rename("CN=" + ADNewName)
                dt.Rows.Add("Success")
                de.Close()
            Else
                Select Case LCase(AccountType)
                    Case "user"
                        Throw New Exception("User does not exist")
                    Case "computer"
                        Throw New Exception("Computer does not exist")
                End Select
            End If

            Return Me.GenerateActivityResult(dt)
        End Function



        Public Function GetAdEntry(ByVal domainServer As String, ByVal domainPort As String, ByVal username As String, ByVal password As String) As DirectoryEntry
            Dim defaultAdSecurePort As String = "636"
            If domainPort.Equals(defaultAdSecurePort) AndAlso IsIpAddress(domainServer) Then Throw New Exception("When using a secure port, a server domain name must be defined for the device.")
            Dim domainUrl As String = "LDAP://" & domainServer

            If Not domainPort.Equals(DefaultAdPort) Then
                domainUrl = domainUrl & ":" & domainPort
            End If

            Dim adEntry = New DirectoryEntry(domainUrl, username, password, AuthenticationTypes.Secure)
            Return adEntry
        End Function

        Private Function IsIpAddress(ByVal domainServer As String) As Boolean
            Dim address As IPAddress
            Return IPAddress.TryParse(domainServer, address)
        End Function



    End Class
End Namespace

