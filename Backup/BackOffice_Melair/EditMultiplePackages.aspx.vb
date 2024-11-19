Public Class EditMultiplePackages
  Inherits System.Web.UI.Page

  Dim sqlAccess As New SQL
  Dim package As New MelairPackage
  Dim flight As New MelairFlight
  Dim prices As New MelairPrices

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    If Session("login") <> "Y" Then
      Response.Redirect("Login.aspx")
    End If

    Page.MaintainScrollPositionOnPostBack = True
    Try
      If Not IsPostBack Then
        package = sqlAccess.retrieveFirstPackage(Session("selPackName"))
        Session("package") = package

        titleLbl.Text = Session("selPackName") & "  -  Tudo"
        completeData()

        If Not IO.Directory.Exists(Server.MapPath("Images/Temp")) Then
          IO.Directory.CreateDirectory(Server.MapPath("Images/Temp"))
        Else
          Dim images() As String = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))
          For Each file In images
            IO.File.Delete(file)
          Next
        End If
        recreateImages()
      End If
    Catch ex As Exception
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" _
                                          & Replace(ex.Message, Environment.NewLine, " ") & "');", True)
    End Try
  End Sub

  Private Sub completeData()
    Dim basicData As Dictionary(Of String, String) = CType(Session("basicData"), Dictionary(Of String, String))

    imagesLbl.Text = ""
    For Each image In package.images
      If IO.File.Exists(Server.MapPath("Images/Uploads/" & package.id & "/" & image)) Then
        imagesLbl.Text &= image & " "
      End If
    Next

    nameField.Text = package.name
    itinNameField.Text = package.itineraryName
    diningField.Text = package.dining
    includedField.Text = package.included
    notIncludedField.Text = package.notIncluded
    notesField.Text = Replace(package.notes, "<br/><br/>", Environment.NewLine())

    Session("itinCode") = package.itineraryCode
    Session("destination") = package.destination
  End Sub

  Private Sub recreateImages()
    package = Session("package")
    Dim packId As String = sqlAccess.getPackageIds(package.name, package.sailingDate)
    Dim missingImages As String = ""

    Dim fileName As String
    For imgIndex As Byte = 0 To 4
      fileName = package.images(imgIndex)
      If fileName <> "" Then
        If IO.File.Exists(Server.MapPath("Images/Uploads/" & packId.Split("|")(0) & "/" & fileName)) Then
          IO.File.Copy(Server.MapPath("Images/Uploads/" & packId.Split("|")(0) & "/" & fileName), _
                       Server.MapPath("Images/Temp/" & fileName))
        Else
          missingImages &= fileName & " "
          package.images(imgIndex) = ""
        End If
      End If
    Next
    If missingImages <> "" Then
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Alerta', '" _
                                          & "As imagens " & missingImages & "não podem ser encontradas. " _
                                          & "');", True)
    End If
    Session("package") = package
  End Sub


  Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "$('#popup').modal('hide');", True)

    If Session("detailPending") = "Y" Then
      Session("detailPending") = Nothing
      'prices.Visible = True
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "showPrices();", True)
    ElseIf Session("linePending") = "Y" Then
      Session("linePending") = Nothing
      
    End If
    If Session("packageModified") = "Y" Then
      Session("packageModified") = ""
      Response.Redirect("Packages.aspx")
    End If

    scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendar", "setCalendars();", True)
  End Sub

  Protected Sub showImages(sender As Object, e As EventArgs) Handles uploadBtn.Click
    package = Session("package")
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showImages", "showImagesPopup('" & package.images(0) & "', '" & package.images(1) _
                                        & "', '" & package.images(2) & "', '" & package.images(3) & "', '" & package.images(4) & "');", True)
  End Sub

  Protected Sub saveImages(sender As Object, e As EventArgs) Handles saveImagesBtn.Click
    package = Session("package")
    Dim fileName As String
    imagesLbl.Text = ""

    If uploadCtrl1.HasFile Then
      fileName = uploadCtrl1.FileName

      If package.images(0) = "" Then
        uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      Else
        Dim currentFile As String = package.images(0)
        If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
          My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
        End If
        uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      End If

      package.images(0) = fileName
    End If
    If uploadCtrl2.HasFile Then
      fileName = uploadCtrl2.FileName

      If package.images(1) = "" Then
        uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      Else
        Dim currentFile As String = package.images(1)
        If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
          My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
        End If
        uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      End If

      package.images(1) = fileName
    End If
    If uploadCtrl3.HasFile Then
      fileName = uploadCtrl3.FileName

      If package.images(2) = "" Then
        uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      Else
        Dim currentFile As String = package.images(2)
        If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
          My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
        End If
        uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      End If

      package.images(2) = fileName
    End If
    If uploadCtrl4.HasFile Then
      fileName = uploadCtrl4.FileName

      If package.images(3) = "" Then
        uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      Else
        Dim currentFile As String = package.images(3)
        If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
          My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
        End If
        uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      End If

      package.images(3) = fileName
    End If
    If uploadCtrl5.HasFile Then
      fileName = uploadCtrl5.FileName

      If package.images(4) = "" Then
        uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      Else
        Dim currentFile As String = package.images(4)
        If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
          My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
        End If
        uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
      End If

      package.images(4) = fileName
    End If

    For Each item In package.images
      If IO.File.Exists(Server.MapPath("Images/Temp/" & item)) Then
        imagesLbl.Text &= item & " "
      End If
    Next
    Session("package") = package

    scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideImages", "$('#imagesPopup').modal('hide');", True)
  End Sub

  Protected Sub deleteImage(sender As Object, e As EventArgs)
    package = Session("package")
    Dim fileName As String = ""
    Dim imgIndex As Byte = CByte(sender.CommandArgument)
    imagesLbl.Text = ""

    Select Case imgIndex
      Case 3
        fileName = package.images(2)
      Case 4
        fileName = package.images(3)
      Case 5
        fileName = package.images(4)
    End Select
    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & fileName))
    package.images(imgIndex - 1) = ""

    For Each image In package.images
      If IO.File.Exists(Server.MapPath("Images/Temp/" & image)) Then
        imagesLbl.Text &= image & " "
      End If
    Next
    Session("package") = package

    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showImages", "showImagesPopup('" & package.images(0) & "', '" & package.images(1) & "', '" & package.images(2) & "', '" _
                                        & package.images(3) & "', '" & package.images(4) & "');", True)
  End Sub

  Private Function createCustomSelector(name As String) As DropDownList
    Dim result As New DropDownList

    If name = "citySelector" Then
      result.Items.Add(New ListItem("Selecione uma cidade", "-1"))
      result.Items.Add(New ListItem("Lisboa (Portugal)", "LIS"))
      result.Items.Add(New ListItem("Porto (Portugal)", "OPO"))
      result.Items.Add(New ListItem("Madrid (Espanha)", "MAD"))
    ElseIf name = "statusSelector" Then
      result.Items.Add(New ListItem("GTD", "GTD"))
      result.Items.Add(New ListItem("RQ", "RQ"))
    ElseIf name = "typeSelector" Then
      result.Items.Add(New ListItem("Antes", "B"))
      result.Items.Add(New ListItem("Depois", "A"))
    End If
    result.Items(0).Selected = True
    result.ID = name

    createCustomSelector = result
  End Function

  Protected Sub onRowDataBound(sender As Object, e As GridViewRowEventArgs)
    If e.Row.RowType = DataControlRowType.DataRow Then
      Dim num As Integer = e.Row.RowIndex
      Dim compSelector As DropDownList = CType(e.Row.FindControl("compSelector"), DropDownList)
      compSelector.Items.Clear()

      compSelector.DataSource = sqlAccess.retrieveAirCompanies()
      compSelector.DataTextField = "Company"
      compSelector.DataValueField = "Company"
      compSelector.DataBind()
      compSelector.Items.FindByText("Tap Air Portugal").Selected = True
    End If
  End Sub

  Protected Sub goBack(sender As Object, e As EventArgs) Handles backBtn.Click
    Response.Redirect("Packages.aspx")
  End Sub

  Private Function transformTime(time As String, Optional fromDB As Char = "N") As String
    Dim result As String = ""

    Dim hour As String = "", min As String = "", mode As String = ""
    If time.Length = 9 Then ' Hora < 10
      hour = time.Substring(0, 1).PadLeft(2, "0")
      min = time.Substring(4, 2)
      mode = time.Substring(7, 2)
    ElseIf time.Length = 10 Then ' Hora >= 10
      hour = time.Substring(0, 2)
      min = time.Substring(5, 2)
      mode = time.Substring(8, 2)
    End If

    If mode = "PM" Then
      Select Case hour
        Case "01"
          hour = "13"
        Case "02"
          hour = "14"
        Case "03"
          hour = "15"
        Case "04"
          hour = "16"
        Case "05"
          hour = "17"
        Case "06"
          hour = "18"
        Case "07"
          hour = "19"
        Case "08"
          hour = "20"
        Case "09"
          hour = "21"
        Case "10"
          hour = "22"
        Case "11"
          hour = "23"
        Case "12"
          hour = "00"
      End Select
    End If

    result = hour & min & "00"
    transformTime = result
  End Function

  Protected Sub registerPackage(sender As Object, e As EventArgs) Handles editRegisterBtn.Click
    Dim packageToUpdate As String = ""
    Dim packageColumnsToUpdate As New Dictionary(Of String, String)

    packageColumnsToUpdate = makePackageColumnsToUpdate()
    packageToUpdate = package.name
    Try
      Dim errorStr As String = ""
      If errorStr = "" Then
        If package.name = "" Then
          errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
        ElseIf package.itineraryName = "" Then
          errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
        ElseIf package.dining = "" Then
          errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
        ElseIf package.included = "" Then
          errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
        ElseIf package.notIncluded = "" Then
          errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
        End If
        If errorStr = "" Then
          sqlAccess.updatePackage(packageColumnsToUpdate, packageToUpdate)
          Dim ids As New List(Of String)
          ids = sqlAccess.getIdsFromEqualPackageName(package.name)
          For Each identification In ids
            If IO.Directory.Exists(Server.MapPath("Images/Uploads/" & identification)) Then
              IO.Directory.Delete(Server.MapPath("Images/Uploads/" & identification), True)
            End If
            My.Computer.FileSystem.CopyDirectory(Server.MapPath("Images/Temp"), _
                                                 Server.MapPath("Images/Uploads/" & identification), True)
          Next
          Session("packageModified") = "Y"
        End If
      End If

      If errorStr = "" Then
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Pacote alterado com êxito.');", True)
      Else
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & errorStr & "');", True)
      End If
    Catch ex As Exception
      Dim msg As String = ""

      msg = Replace(ex.Message, Environment.NewLine, " ")
      msg = Replace(ex.Message, "'", " ")
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & msg & "');", True)
    End Try
  End Sub

  Protected Sub showEditPrices(sender As Object, e As EventArgs) Handles editPricesBtn.Click
    package = Session("package")
    flight = sqlAccess.getMelairFlight(package.id)
    prices = sqlAccess.getMelairPrices(flight.id)
    fillPricesModal(prices)
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "$('#pricesModal').modal('show');", True)
    Session("prices") = prices
  End Sub

  Public Sub fillPricesModal(prices As MelairPrices)
    For iRow As Integer = 1 To 4
      For jCell As Integer = 1 To 6
        Select Case iRow
          Case 1
            CType(FindControl("tInt" & jCell), TextBox).Text = prices.priceInterior(jCell - 1)
          Case 2
            CType(FindControl("tExt" & jCell), TextBox).Text = prices.priceExterior(jCell - 1)
          Case 3
            CType(FindControl("tBal" & jCell), TextBox).Text = prices.priceBalcony(jCell - 1)
          Case 4
            CType(FindControl("tSui" & jCell), TextBox).Text = prices.priceSuite(jCell - 1)
        End Select
      Next
    Next
    CType(FindControl("tInt7"), TextBox).Text = prices.offerInterior
    CType(FindControl("tExt7"), TextBox).Text = prices.offerExterior
    CType(FindControl("tBal7"), TextBox).Text = prices.offerBalcony
    CType(FindControl("tSui7"), TextBox).Text = prices.offerSuite
  End Sub

  Public Sub fillPricesSession(prices As MelairPrices)
    For iRow As Integer = 1 To 4
      For jCell As Integer = 1 To 6
        Select Case iRow
          Case 1
            CType(FindControl("tInt" & jCell), TextBox).Text = prices.priceInterior(iRow)
          Case 2
            CType(FindControl("tExt" & jCell), TextBox).Text = prices.priceExterior(iRow)
          Case 3
            CType(FindControl("tBal" & jCell), TextBox).Text = prices.priceBalcony(iRow)
          Case 4
            CType(FindControl("tSui" & jCell), TextBox).Text = prices.priceSuite(iRow)
        End Select
      Next
    Next
    CType(FindControl("tInt7"), TextBox).Text = prices.offerInterior
    CType(FindControl("tExt7"), TextBox).Text = prices.offerExterior
    CType(FindControl("tBal7"), TextBox).Text = prices.offerBalcony
    CType(FindControl("tSui7"), TextBox).Text = prices.offerSuite
  End Sub

  Protected Sub showEditFlight(sender As Object, e As EventArgs) Handles editFlightBtn.Click
    package = Session("package")
    flight = sqlAccess.getMelairFlight(package.id)
    fillFlightsModal(flight)
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showFlight", "$('#flightModal').modal('show');", True)
  End Sub

  Protected Sub fillFlightsModal(flight As MelairFlight)
    listFlightCompany.Items(0).Text = flight.company
    imageFlightLogo.ImageUrl = "Images/Logos/" & sqlAccess.getCompanyLogo(flight.company)
    listFlightCity.Items(0).Text = flight.city
    textFlightDescription.Text = flight.description
    textFlightCabinsPerCity.Text = flight.cabins
    listFlightStatus.Items(0).Text = flight.status
    textFlightCode.Text = flight.code
    textFlightTime.Text = flight.time
    textFlightTaxes.Text = flight.taxes
    textFlightReturn.Text = flight.returnFlight
    textFlightReturnTime.Text = flight.returnFlightTime
    textFlightTransfer.Text = flight.transfer
    textFlightHotel.Text = flight.hotel
    textFlightOfferDescription.Text = flight.offer
    Session("flight") = flight
  End Sub

  Protected Sub fillFlightsSession(flight As MelairFlight)
    flight.company = listFlightCompany.SelectedItem.Text
    flight.logo = sqlAccess.getCompanyLogo(flight.company)
    flight.city = listFlightCity.SelectedItem.Text
    flight.description = textFlightDescription.Text
    flight.cabins = textFlightCabinsPerCity.Text
    flight.status = listFlightStatus.Items(0).Text
    flight.code = textFlightCode.Text
    flight.time = textFlightTime.Text
    flight.taxes = textFlightTaxes.Text
    flight.returnFlight = textFlightReturn.Text
    flight.returnFlightTime = textFlightReturnTime.Text
    flight.transfer = textFlightTransfer.Text
    flight.hotel = textFlightHotel.Text
    flight.offer = textFlightOfferDescription.Text
    Session("flight") = flight
  End Sub

  Protected Sub saveFlight(sender As Object, e As EventArgs)
    Dim ids As List(Of String)
    Dim flightColumnsToUpdate As New Dictionary(Of String, String)

    Try
      package = Session("package")
      fillFlightsSession(flight)
      flightColumnsToUpdate = makeFlightColumnsToUpdate()
      ids = sqlAccess.getIdsFromEqualPackageName(package.name)
      sqlAccess.updateFlights(flightColumnsToUpdate, ids)
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Vôo alterado com êxito.');", True)
    Catch ex As Exception
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & Replace(ex.Message, Environment.NewLine, " ") & "');", True)
    End Try
  End Sub

  Protected Sub savePrices(sender As Object, e As EventArgs)
    Dim idsPackages As List(Of String)
    Dim idsFlights As List(Of String)
    Dim pricesColumnsToUpdate As New Dictionary(Of String, String)

    Try
      package = Session("package")
      pricesColumnsToUpdate = makePricesColumnsToUpdate()
      idsPackages = sqlAccess.getIdsFromEqualPackageName(package.name)
      idsFlights = sqlAccess.getIdsFlightFromIdsPackages(idsPackages)
      sqlAccess.updatePrices(pricesColumnsToUpdate, idsFlights)
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Price alterado com êxito.');", True)
    Catch ex As Exception
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & Replace(ex.Message, Environment.NewLine, " ") & "');", True)
    End Try
  End Sub

  Private Function makePricesColumnsToUpdate() As Dictionary(Of String, String)
    Dim pricesColumnsToUpdate As New Dictionary(Of String, String)

    prices = Session("prices")
    If cInt1.Checked Then
      prices.priceInterior(0) = Trim(tInt1.Text)
      pricesColumnsToUpdate.Add("IDouble", prices.priceInterior(0))
    End If
    If cInt2.Checked Then
      prices.priceInterior(1) = Trim(tInt2.Text)
      pricesColumnsToUpdate.Add("ITriCua", prices.priceInterior(1))
    End If
    If cInt3.Checked Then
      prices.priceInterior(2) = Trim(tInt3.Text)
      pricesColumnsToUpdate.Add("I34Ad", prices.priceInterior(2))
    End If
    If cInt4.Checked Then
      prices.priceInterior(3) = Trim(tInt4.Text)
      pricesColumnsToUpdate.Add("I34Cr", prices.priceInterior(3))
    End If
    If cInt5.Checked Then
      prices.priceInterior(4) = Trim(tInt5.Text)
      pricesColumnsToUpdate.Add("I34Bb", prices.priceInterior(4))
    End If
    If cInt6.Checked Then
      prices.priceInterior(5) = Trim(tInt6.Text)
      pricesColumnsToUpdate.Add("IInd", prices.priceInterior(5))
    End If
    If cInt7.Checked Then
      prices.offerInterior = Trim(tInt7.Text)
      pricesColumnsToUpdate.Add("IOffer", prices.offerInterior)
    End If
    If cExt1.Checked Then
      prices.priceExterior(0) = Trim(tExt1.Text)
      pricesColumnsToUpdate.Add("ODouble", prices.priceExterior(0))
    End If
    If cExt2.Checked Then
      prices.priceExterior(1) = Trim(tExt2.Text)
      pricesColumnsToUpdate.Add("OTriCua", prices.priceExterior(1))
    End If
    If cExt3.Checked Then
      prices.priceExterior(2) = Trim(tExt3.Text)
      pricesColumnsToUpdate.Add("O34Ad", prices.priceExterior(2))
    End If
    If cExt4.Checked Then
      prices.priceExterior(3) = Trim(tExt4.Text)
      pricesColumnsToUpdate.Add("O34Cr", prices.priceExterior(3))
    End If
    If cExt5.Checked Then
      prices.priceExterior(4) = Trim(tExt5.Text)
      pricesColumnsToUpdate.Add("O34Bb", prices.priceExterior(4))
    End If
    If cExt6.Checked Then
      prices.priceExterior(5) = Trim(tExt6.Text)
      pricesColumnsToUpdate.Add("OInd", prices.priceExterior(5))
    End If
    If cExt7.Checked Then
      prices.offerExterior = Trim(tExt7.Text)
      pricesColumnsToUpdate.Add("OOffer", prices.offerExterior)
    End If
    If cBal1.Checked Then
      prices.priceBalcony(0) = Trim(tBal1.Text)
      pricesColumnsToUpdate.Add("BDouble", prices.priceBalcony(0))
    End If
    If cBal2.Checked Then
      prices.priceBalcony(1) = Trim(tBal2.Text)
      pricesColumnsToUpdate.Add("BTriCua", prices.priceBalcony(1))
    End If
    If cBal3.Checked Then
      prices.priceBalcony(2) = Trim(tBal3.Text)
      pricesColumnsToUpdate.Add("B34Ad", prices.priceBalcony(2))
    End If
    If cBal4.Checked Then
      prices.priceBalcony(3) = Trim(tBal4.Text)
      pricesColumnsToUpdate.Add("B34Cr", prices.priceBalcony(3))
    End If
    If cBal5.Checked Then
      prices.priceBalcony(4) = Trim(tBal5.Text)
      pricesColumnsToUpdate.Add("B34Bb", prices.priceBalcony(4))
    End If
    If cBal6.Checked Then
      prices.priceBalcony(5) = Trim(tBal6.Text)
      pricesColumnsToUpdate.Add("BInd", prices.priceBalcony(5))
    End If
    If cBal7.Checked Then
      prices.offerBalcony = Trim(tBal7.Text)
      pricesColumnsToUpdate.Add("BOffer", prices.offerBalcony)
    End If
    If cSui1.Checked Then
      prices.priceSuite(0) = Trim(tSui1.Text)
      pricesColumnsToUpdate.Add("SDouble", prices.priceSuite(0))
    End If
    If cSui2.Checked Then
      prices.priceSuite(1) = Trim(tSui2.Text)
      pricesColumnsToUpdate.Add("STriCua", prices.priceSuite(1))
    End If
    If cSui3.Checked Then
      prices.priceSuite(2) = Trim(tSui3.Text)
      pricesColumnsToUpdate.Add("S34Ad", prices.priceSuite(2))
    End If
    If cSui4.Checked Then
      prices.priceSuite(3) = Trim(tSui4.Text)
      pricesColumnsToUpdate.Add("S34Cr", prices.priceSuite(3))
    End If
    If cSui5.Checked Then
      prices.priceSuite(4) = Trim(tSui5.Text)
      pricesColumnsToUpdate.Add("S34Bb", prices.priceSuite(4))
    End If
    If cSui6.Checked Then
      prices.priceSuite(5) = Trim(tSui6.Text)
      pricesColumnsToUpdate.Add("SInd", prices.priceSuite(5))
    End If
    If cSui7.Checked Then
      prices.offerSuite = Trim(tSui7.Text)
      pricesColumnsToUpdate.Add("SOffer", prices.offerSuite)
    End If
    Session("prices") = prices

    Return pricesColumnsToUpdate
  End Function

  Private Function makeFlightColumnsToUpdate() As Dictionary(Of String, String)
    Dim flightColumnsToUpdate As New Dictionary(Of String, String)

    flight = Session("flight")
    If selectFlightCompany.Checked Then
      flight.company = Trim(listFlightCompany.SelectedItem.Text)
      flightColumnsToUpdate.Add("Company", flight.company)
      flight.logo = Trim(sqlAccess.getCompanyLogo(flight.company))
      flightColumnsToUpdate.Add("Logo", flight.logo)
    End If
    If selectFlightDescription.Checked Then
      flight.description = Trim(textFlightDescription.Text)
      flightColumnsToUpdate.Add("Description", flight.description)
    End If
    If selectFlightCity.Checked Then
      flight.city = Trim(listFlightCity.SelectedItem.Text)
      flightColumnsToUpdate.Add("City", flight.city)
    End If
    If selectFlightCode.Checked Then
      flight.code = Trim(textFlightCode.Text)
      flightColumnsToUpdate.Add("FlightCode", flight.code)
    End If
    If selectFlightTime.Checked Then
      flight.time = Trim(textFlightTime.Text)
      flightColumnsToUpdate.Add("FlightTime", flight.time)
    End If
    If selectFlightTaxes.Checked Then
      flight.taxes = Trim(textFlightTaxes.Text)
      flightColumnsToUpdate.Add("FlightTaxes", flight.taxes)
    End If
    If selectFlightReturn.Checked Then
      flight.returnFlight = Trim(textFlightReturn.Text)
      flightColumnsToUpdate.Add("ReturnFlight", flight.returnFlight)
    End If
    If selectFlightReturnTime.Checked Then
      flight.returnFlightTime = Trim(textFlightReturnTime.Text)
      flightColumnsToUpdate.Add("ReturnFlightTime", flight.returnFlightTime)
    End If
    If selectFlightTransfer.Checked Then
      flight.transfer = Trim(textFlightTransfer.Text)
      flightColumnsToUpdate.Add("FlightTransfer", flight.transfer)
    End If
    If selectFlightHotel.Checked Then
      flight.hotel = Trim(textFlightHotel.Text)
      flightColumnsToUpdate.Add("FlightHotel", flight.hotel)
    End If
    If selectFlightOfferDescription.Checked Then
      flight.offer = Trim(textFlightOfferDescription.Text)
      flightColumnsToUpdate.Add("OfferDescription", flight.offer)
    End If
    If selectFlightCabinsPerCity.Checked Then
      flight.cabins = Trim(textFlightCabinsPerCity.Text)
      flightColumnsToUpdate.Add("CabinsPerCity", flight.cabins)
    End If
    If selectFlightStatus.Checked Then
      flight.status = Trim(listFlightStatus.SelectedItem.Text)
      flightColumnsToUpdate.Add("FlightStatus", flight.status)
    End If

    Return flightColumnsToUpdate
  End Function


  Private Function makePackageColumnsToUpdate() As Dictionary(Of String, String)
    Dim packageColumnsToUpdate As New Dictionary(Of String, String)

    package = Session("package")
    If selectItineraryName.Checked Then
      package.itineraryName = Trim(itinNameField.Text)
      packageColumnsToUpdate.Add("ItineraryName", package.itineraryName)
    End If
    If selectDining.Checked Then
      package.dining = Trim(diningField.Text)
      packageColumnsToUpdate.Add("Dining", package.dining)
    End If
    If selectIncluded.Checked Then
      package.included = Trim(includedField.Text)
      packageColumnsToUpdate.Add("Included", package.included)
    End If
    If selectNotIncluded.Checked Then
      package.notIncluded = Trim(notIncludedField.Text)
      packageColumnsToUpdate.Add("NotIncluded", package.notIncluded)
    End If
    If selectNotes.Checked Then
      package.notes = Trim(notesField.Text)
      packageColumnsToUpdate.Add("Notes", package.notes)
    End If
    If selectImages.Checked Then
      Dim i As Integer = 1
      For Each image In package.images
        packageColumnsToUpdate.Add("Img" & i, image)
        i += 1
      Next
    End If

    Return packageColumnsToUpdate
  End Function

  Public Sub selectAll(sender As Object, eventBtn As EventArgs) Handles selectAllBtn.Click
    selectInt(sender, eventBtn)
    selectExt(sender, eventBtn)
    selectBal(sender, eventBtn)
    selectSui(sender, eventBtn)
  End Sub

  Public Sub unselectAll(sender As Object, eventBtn As EventArgs) Handles unselectAllBtn.Click
    selectInt(sender, eventBtn)
    selectExt(sender, eventBtn)
    selectBal(sender, eventBtn)
    selectSui(sender, eventBtn)
  End Sub

  Public Sub selectInt(sender As Object, eventBtn As EventArgs) Handles selectIntBtn.Click
    Dim check As Boolean = True

    If sender.CommandArgument.Equals("False") Then
      check = False
    End If
    cInt1.Checked = check
    cInt2.Checked = check
    cInt3.Checked = check
    cInt4.Checked = check
    cInt5.Checked = check
    cInt6.Checked = check
    cInt7.Checked = check
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "$('#pricesModal').modal('show');", True)
  End Sub

  Public Sub selectExt(sender As Object, eventBtn As EventArgs) Handles selectExtBtn.Click
    Dim check As Boolean = True

    If sender.CommandArgument.Equals("False") Then
      check = False
    End If
    cExt1.Checked = check
    cExt2.Checked = check
    cExt3.Checked = check
    cExt4.Checked = check
    cExt5.Checked = check
    cExt6.Checked = check
    cExt7.Checked = check
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "$('#pricesModal').modal('show');", True)
  End Sub

  Public Sub selectBal(sender As Object, eventBtn As EventArgs) Handles selectBalBtn.Click
    Dim check As Boolean = True

    If sender.CommandArgument.Equals("False") Then
      check = False
    End If
    cBal1.Checked = check
    cBal2.Checked = check
    cBal3.Checked = check
    cBal4.Checked = check
    cBal5.Checked = check
    cBal6.Checked = check
    cBal7.Checked = check
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "$('#pricesModal').modal('show');", True)
  End Sub

  Public Sub selectSui(sender As Object, eventBtn As EventArgs) Handles selectSuiBtn.Click
    Dim check As Boolean = True

    If sender.CommandArgument.Equals("False") Then
      check = False
    End If
    cSui1.Checked = check
    cSui2.Checked = check
    cSui3.Checked = check
    cSui4.Checked = check
    cSui5.Checked = check
    cSui6.Checked = check
    cSui7.Checked = check
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "$('#pricesModal').modal('show');", True)
  End Sub

  Public Sub refreshLogo()
    Dim company As String = ""

    company = listFlightCompany.SelectedItem.Text
    imageFlightLogo.ImageUrl = "Images/Logos/" & sqlAccess.getCompanyLogo(company)
    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showFlight", "$('#flightModal').modal('show');", True)
  End Sub

End Class

