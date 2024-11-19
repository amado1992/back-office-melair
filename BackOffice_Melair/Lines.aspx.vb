Public Class Lines
    Inherits System.Web.UI.Page
    Dim sqlAccess As New SQL
    Dim paramIdPack As String = 0
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Session IsNot Nothing) Then
            paramIdPack = Session("IdPack")
        End If

        If (Session IsNot Nothing) Then
            Dim checkedFlight = Session("CheckedFlight")
            If checkedFlight = True Then
                idCheckVoos.Style.Remove("display")
            End If
        End If

        If (Session IsNot Nothing) Then
            Dim checkedTransfer = Session("CheckedTransfer")
            If checkedTransfer = True Then
                idCheckTransferes.Style.Remove("display")
            End If
        End If

        If (Session IsNot Nothing) Then
            Dim checkedHotel = Session("CheckedHotel")
            If checkedHotel = True Then
                idCheckHotel.Style.Remove("display")
            End If
        End If

        If Not IsPostBack Then
            programLine.Items.Add(New ListItem("1", 1))
            programLine.Items.Add(New ListItem("2", 2))
            programLine.Items.Add(New ListItem("3", 3))
            programLine.Items.Add(New ListItem("4", 4))
            programLine.Items.Add(New ListItem("5", 5))
            programLine.Items.Add(New ListItem("6", 6))
            programLine.Items.Add(New ListItem("7", 7))
            programLine.Items.Add(New ListItem("8", 8))
            programLine.Items.Add(New ListItem("9", 9))
            programLine.Items.Add(New ListItem("10", 10))
            programLine.Items.Add(New ListItem("11", 11))
            programLine.Items.Add(New ListItem("12", 12))
            programLine.Items.Add(New ListItem("13", 13))
            programLine.Items.Add(New ListItem("14", 14))
            programLine.Items.Add(New ListItem("15", 15))
            programLine.Items.Add(New ListItem("16", 16))
            programLine.Items.Add(New ListItem("17", 17))
            programLine.Items.Add(New ListItem("18", 18))
            programLine.Items.Add(New ListItem("19", 19))
            programLine.Items.Add(New ListItem("20", 20))

            afterBefore.Items.Add(New ListItem("Antes", 1))
            afterBefore.Items.Add(New ListItem("Depois", 0))

            lineType.Items.Add(New ListItem("Vuelo", "V"))
            lineType.Items.Add(New ListItem("Hotel", "H"))
            lineType.Items.Add(New ListItem("Transfer", "T"))

            BindGrid()

        End If

    End Sub

    Public Sub insert()
        Dim idPack, _lineType, description, num, before
        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        idPack = paramIdPack
        'idPack = 983
        _lineType = lineType.Text
        description = descripTextBox.Text
        num = programLine.Text
        before = afterBefore.Text

        basicData.Add("IdPack", idPack)
        basicData.Add("LineType", _lineType)
        basicData.Add("Description", description)
        basicData.Add("Num", num)

        If (before = 1) Then
            basicData.Add("Before", 1)
        End If

        If (before = 0) Then
            basicData.Add("Before", 0)
        End If

        packageData.SetValue(basicData, 0)

        sqlAccess.insertLine(packageData)

        idPack = String.Empty
        lineType.Text = -1
        descripTextBox.Text = String.Empty
        afterBefore.Text = -1
        programLine.Text = -1

        Me.BindGrid()

    End Sub

    Private Sub BindGrid()
        Dim dbData1 As DataTable = sqlAccess.retrieveLines("T", paramIdPack)
        If (Not dbData1 Is Nothing) And (dbData1.Rows.Count > 0) Then
            GridView1.DataSource = dbData1
            GridView1.DataBind()
        End If

        Dim dbData2 As DataTable = sqlAccess.retrieveLines("H", paramIdPack)
        If (Not dbData2 Is Nothing) And (dbData2.Rows.Count > 0) Then

            GridView2.DataSource = dbData2
            GridView2.DataBind()
        End If

        Dim dbData3 As DataTable = sqlAccess.retrieveLines("V", paramIdPack)
        If (Not dbData3 Is Nothing) And (dbData3.Rows.Count > 0) Then

            GridView3.DataSource = dbData3
            GridView3.DataBind()
        End If
    End Sub

    Public Sub packages(sender As Object, e As EventArgs) Handles btPackages.Click
        Response.Redirect("Packages.aspx")
    End Sub

    Public Sub cabins(sender As Object, e As EventArgs) Handles btCabins.Click
        Response.Redirect("Cabin.aspx")
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim bf As BoundField = DirectCast(GridView1.Columns(2), BoundField)
            Dim cell As TableCell = e.Row.Cells(GridView1.Columns.IndexOf(bf))

            ' Aquí puedes comprobar una condición y cambiar el contenido de la celda en consecuencia
            If cell.Text = "True" Then
                cell.Text = "Antes"
            End If

            If cell.Text = "False" Then
                cell.Text = "Depois"
            End If
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim bf As BoundField = DirectCast(GridView2.Columns(2), BoundField)
            Dim cell As TableCell = e.Row.Cells(GridView2.Columns.IndexOf(bf))

            ' Aquí puedes comprobar una condición y cambiar el contenido de la celda en consecuencia
            If cell.Text = "True" Then
                cell.Text = "Antes"
            End If

            If cell.Text = "False" Then
                cell.Text = "Depois"
            End If
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim bf As BoundField = DirectCast(GridView3.Columns(2), BoundField)
            Dim cell As TableCell = e.Row.Cells(GridView3.Columns.IndexOf(bf))

            ' Aquí puedes comprobar una condición y cambiar el contenido de la celda en consecuencia
            If cell.Text = "True" Then
                cell.Text = "Antes"
            End If

            If cell.Text = "False" Then
                cell.Text = "Depois"
            End If
        End If
    End Sub
End Class