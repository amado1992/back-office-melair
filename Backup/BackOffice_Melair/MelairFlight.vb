Public Class MelairFlight
  Public Property id As String = ""
  Public Property idPack As String = ""
  Public Property description As String = ""
  Public Property company As String = ""
  Public Property logo As String = ""
  Public Property city As String = ""
  Public Property code As String = ""
  Public Property dateFlight As String = ""
  Public Property time As String = ""
  Public Property taxes As String = ""
  Public Property returnFlight As String = ""
  Public Property returnFlightTime As String = ""
  Public Property transfer As String = ""
  Public Property hotel As String = ""
  Public Property offer As String = ""
  Public Property cabins As String = ""
  Public Property status As String = ""

  Public Sub New(row As DataRow)
    id = row.Item("idFlight")
    idPack = row.Item("idPack")
    description = row.Item("Description")
    company = row.Item("Company")
    logo = row.Item("Logo")
    city = row.Item("City")
    code = row.Item("FlightCode")
    dateFlight = row.Item("FlightDate")
    time = row.Item("FlightTime")
    taxes = row.Item("FlightTaxes")
    returnFlight = row.Item("ReturnFlight")
    returnFlightTime = row.Item("ReturnFlightTime")
    transfer = row.Item("FlightTransfer")
    hotel = row.Item("FlightHotel")
    offer = row.Item("OfferDescription")
    cabins = row.Item("CabinsPerCity")
    status = row.Item("FlightStatus")
  End Sub

  Public Sub New()

  End Sub
End Class
