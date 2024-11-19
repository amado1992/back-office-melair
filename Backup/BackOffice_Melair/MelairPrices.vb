Public Class MelairPrices
  Public ReadOnly id As String = ""
  Public Property priceInterior As New List(Of String)
  Public Property priceExterior As New List(Of String)
  Public Property priceBalcony As New List(Of String)
  Public Property priceSuite As New List(Of String)
  Public Property offerInterior As String = ""
  Public Property offerExterior As String = ""
  Public Property offerBalcony As String = ""
  Public Property offerSuite As String = ""

  Public Sub New(row As DataRow)
    id = row.Item("idFlight")
    priceInterior.Add(row.Item("IDouble").ToString)
    priceInterior.Add(row.Item("ITriCua"))
    priceInterior.Add(row.Item("I34Ad"))
    priceInterior.Add(row.Item("I34Cr"))
    priceInterior.Add(row.Item("I34Bb"))
    priceInterior.Add(row.Item("IInd"))
    offerInterior = row.Item("IOffer")
    priceExterior.Add(row.Item("ODouble"))
    priceExterior.Add(row.Item("OTriCua"))
    priceExterior.Add(row.Item("O34Ad"))
    priceExterior.Add(row.Item("O34Cr"))
    priceExterior.Add(row.Item("O34Bb"))
    priceExterior.Add(row.Item("OInd"))
    offerExterior = row.Item("OOffer")
    priceBalcony.Add(row.Item("BDouble"))
    priceBalcony.Add(row.Item("BTriCua"))
    priceBalcony.Add(row.Item("B34Ad"))
    priceBalcony.Add(row.Item("B34Cr"))
    priceBalcony.Add(row.Item("B34Bb"))
    priceBalcony.Add(row.Item("BInd"))
    offerBalcony = row.Item("BOffer")
    priceSuite.Add(row.Item("SDouble"))
    priceSuite.Add(row.Item("STriCua"))
    priceSuite.Add(row.Item("S34Ad"))
    priceSuite.Add(row.Item("S34Cr"))
    priceSuite.Add(row.Item("S34Bb"))
    priceSuite.Add(row.Item("SInd"))
    offerSuite = row.Item("SOffer")
  End Sub

  Public Sub New()

  End Sub

End Class
