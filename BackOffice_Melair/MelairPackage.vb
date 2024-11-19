Public Class MelairPackage

    Public Property id As String = ""
    Public Property name As String = ""
    Public Property shipCode As String = ""
    Public Property sailingDate As String = ""
    Public Property itineraryName As String = ""
    Public Property itineraryCode As String = ""
    Public Property destination As String = ""
    Public Property images As New List(Of String)
    Public Property dining As String = ""
    Public Property included As String = ""
    Public Property notIncluded As String = ""
    Public Property notes As String = ""

    Public Property Flight As Boolean = False
    Public Property Transfer As Boolean = False
    Public Property Hotel As Boolean = False

    Public Property ShipDescription As String = ""
    Public Sub New()

    End Sub

End Class
