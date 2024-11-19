Public Class FlightDetailsDiv

    Public Property flightCode As String = ""
    Public Property flightDate As String = ""
    Public Property flightHour As String = ""
    Public Property flightTaxes As String = ""
    Public Property flightReturn As String = ""
    Public Property flightReturnTime As String = ""
    Public Property flightTransfer As String = ""
    Public Property flightHotel As String = ""
    Public Property flightOffer As String = ""
    Public Property flightDetailsPrices As FlightDetailsPrices

    Public Sub New(ByVal htmlControl As HtmlGenericControl)
        flightCode = CType(htmlControl.FindControl("flightCode"), TextBox).Text
        flightDate = CType(htmlControl.FindControl("flightDate"), TextBox).Text
        flightHour = CType(htmlControl.FindControl("flightHour"), TextBox).Text
        flightTaxes = CType(htmlControl.FindControl("flightTaxes"), TextBox).Text
        flightReturn = CType(htmlControl.FindControl("returnFlight"), TextBox).Text
        flightReturnTime = CType(htmlControl.FindControl("returnFlightTime"), TextBox).Text
        flightTransfer = CType(htmlControl.FindControl("flightTransfer"), TextBox).Text
        flightHotel = CType(htmlControl.FindControl("flightHotel"), TextBox).Text
        flightOffer = CType(htmlControl.FindControl("offerField"), TextBox).Text

        flightDetailsPrices = New FlightDetailsPrices(CType(htmlControl.FindControl("pricesTbl"), Table))
    End Sub

    Public Sub New()

    End Sub

End Class
