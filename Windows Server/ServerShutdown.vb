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

Namespace Ayehu.Sdk.ActivityCreation
    Public Class ActivityClass
        Implements IActivity


        Public HostName As String
        Public UserName As String
        Public Password As String
        Public ActionType As String

        Public Function Execute() As ICustomActivityResult Implements IActivity.Execute

            Dim dt As DataTable = New DataTable("resultSet")
            dt.Columns.Add("Result", GetType(String))


            Dim connectionOptions As ConnectionOptions = New ConnectionOptions
            connectionOptions.Username = UserName
            connectionOptions.Password = Password
            connectionOptions.Authentication = AuthenticationLevel.PacketPrivacy
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate
            connectionOptions.EnablePrivileges = True

            Dim oms As Management.ManagementScope

            If LCase(HostName) = "localhost" OrElse HostName = "127.0.0.1" Then
                oms = New ManagementScope("\\.\root/cimv2")
            Else
                oms = New ManagementScope("\\" & HostName & "\root/cimv2", connectionOptions)
            End If

            Dim oQuery As ObjectQuery = New System.Management.ObjectQuery("Select * from Win32_OperatingSystem")
            Dim oSearcher As ManagementObjectSearcher = New ManagementObjectSearcher(oms, oQuery)
            Dim oReturnCollection As ManagementObjectCollection = oSearcher.Get()

            Select Case ActionType
                Case "Normal"
                    Dim TheActionType() As Object = {1}
                    For Each oReturn As ManagementObject In oReturnCollection
                        oReturn.InvokeMethod("Win32Shutdown", TheActionType)
                    Next
                Case "Forced"
                    Dim TheActionType() As Object = {5}
                    For Each oReturn As ManagementObject In oReturnCollection
                        oReturn.InvokeMethod("Win32Shutdown", TheActionType)
                    Next
                Case "Power off"
                    Dim TheActionType() As Object = {8}
                    For Each oReturn As ManagementObject In oReturnCollection
                        oReturn.InvokeMethod("Win32Shutdown", TheActionType)
                    Next
                Case "Power off forced"
                    Dim TheActionType() As Object = {12}
                    For Each oReturn As ManagementObject In oReturnCollection
                        oReturn.InvokeMethod("Win32Shutdown", TheActionType)
                    Next
            End Select


            dt.Rows.Add("Success")

            Return Me.GenerateActivityResult(dt)

        End Function


    End Class
End Namespace

