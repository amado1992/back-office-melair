Public Class FlightDetailsPrices

  Public Property priceInterior As New List(Of String)
  Public Property priceExterior As New List(Of String)
  Public Property priceBalcony As New List(Of String)
  Public Property priceSuite As New List(Of String)
  Public Property offerInterior As String = ""
  Public Property offerExterior As String = ""
  Public Property offerBalcony As String = ""
  Public Property offerSuite As String = ""

  Public Sub New(ByVal table As Table)
    'Por favor, si se actualiza la estructura de la tabla actualizad también los comentarios
    'del objeto.
    'Al objeto se le pasa la tabla "pricesTbl" con tiene una serie de TextBoxes para introducir
    'los precios. Los precios se dividen en 4 tipos: Interior, Exterior, Balcón y Suite.
    'Actualmente hay 6 precios diferentes para cada tipo, ej: Ocupación dupla 1ª e 2ª cama
    'En la última posición de cada fila se guarda la oferta de cada tipo.
    'En la primera posición de cada fila o columna se guarda un etiqueta informativa para el usuario
    'Ej: Exterior, 3ª e 4ª cama adultos, etc.
    'Por eso se tienen que ignorar las primeras posiciones.

    'No se cuentan las primeras posiciones
    Dim numFilas = table.Rows.Count - 1
    Dim numColumnas = table.Rows(0).Cells.Count - 1

    'Se recorre la tabla sin tener en cuenta las ofertas
    For rowIndex As Integer = 1 To (numFilas)
      For cellIndex As Integer = 1 To (numColumnas - 1)
        Select Case rowIndex
          Case 1
            priceInterior.Add(Replace(Replace(Replace(CType(table.FindControl("int" & cellIndex), TextBox).Text, "€", ""), ".", ""), ",", ""))
          Case 2
            priceExterior.Add(Replace(Replace(Replace(CType(table.FindControl("ext" & cellIndex), TextBox).Text, "€", ""), ".", ""), ",", ""))
          Case 3
            priceBalcony.Add(Replace(Replace(Replace(CType(table.FindControl("bal" & cellIndex), TextBox).Text, "€", ""), ".", ""), ",", ""))
          Case 4
            priceSuite.Add(Replace(Replace(Replace(CType(table.FindControl("su" & cellIndex), TextBox).Text, "€", ""), ".", ""), ",", ""))
        End Select
      Next
    Next

    offerInterior = CType(table.FindControl("int" & numColumnas), TextBox).Text
    offerExterior = CType(table.FindControl("ext" & numColumnas), TextBox).Text
    offerBalcony = CType(table.FindControl("bal" & numColumnas), TextBox).Text
    offerSuite = CType(table.FindControl("su" & numColumnas), TextBox).Text

  End Sub

  Public Sub New()

  End Sub

End Class
