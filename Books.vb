Imports System.Data.OleDb
Public Class Books
    Dim con As New OleDbConnection
    Dim cmd As OleDbCommand
    Dim dt As New DataTable
    Dim adap As New OleDbDataAdapter
    Dim key As Integer = 0 ' Variable to track which record to update or delete

    Private Sub btnBUsers_Click(sender As Object, e As EventArgs) Handles btnBUsers.Click
        Dim usermain As New Users()
        Users.Show()
        Me.Close()
    End Sub

    Private Sub Books_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        con.ConnectionString = "provider=microsoft.jet.oledb.4.0;data source=" & Application.StartupPath & "\Database\dbase.mdb"
        LoadBooksData() ' Load books when the form loads
    End Sub

    Private Sub LoadBooksData()
        Try
            ' Clear existing data in DataTable
            dt.Clear()
            con.Open()
            adap = New OleDbDataAdapter("SELECT * FROM books", con)
            adap.Fill(dt)
            dgBooksList.DataSource = dt
            con.Close()
        Catch ex As Exception
            MsgBox("Error loading books: " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnBSave_Click(sender As Object, e As EventArgs) Handles btnBSave.Click
        Try
            If txtBTitle.Text.Trim = "" Or txtAuthor.Text.Trim = "" Or cboCategories.Text.Trim = "" Or txtQuantity.Text.Trim = "" Or txtPrice.Text.Trim = "" Then
                MsgBox("Please fill in all the fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim quantity As Integer
            If Not Integer.TryParse(txtQuantity.Text, quantity) Then
                MsgBox("Invalid quantity, please enter a number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim price As Decimal
            If Not Decimal.TryParse(txtPrice.Text, price) Then
                MsgBox("Invalid price, please enter a valid decimal number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Insert the book into the database
            con.Open()
            cmd = New OleDbCommand("insert into books([Book Id], [Book Title], [Author], [Category], [Quantity], [Price]) VALUES (?, ?, ?, ?, ?)", con)
            cmd.Parameters.AddWithValue("?", txtID.Text)
            cmd.Parameters.AddWithValue("?", txtBTitle.Text)
            cmd.Parameters.AddWithValue("?", txtAuthor.Text)
            cmd.Parameters.AddWithValue("?", cboCategories.Text)
            cmd.Parameters.AddWithValue("?", quantity)
            cmd.Parameters.AddWithValue("?", price)
            cmd.ExecuteNonQuery()
            con.Close()

            MsgBox("Record saved successfully!", MessageBoxButtons.OK, MessageBoxIcon.Information)


            LoadBooksData()

        Catch ex As Exception
            MsgBox("Error: " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnBLogout_Click(sender As Object, e As EventArgs) Handles btnBLogout.Click
        Dim loginForm As New Form1()
        Form1.Show()
        MsgBox("User successfully logged out!")
        Me.Close()
    End Sub

    Private Sub dgBooksList_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgBooksList.CellContentClick
        If dgBooksList.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = dgBooksList.SelectedRows(0)

            Try
                txtID.Text = selectedRow.Cells("Book Id").Value.ToString()
                txtBTitle.Text = selectedRow.Cells("Book Title").Value.ToString()
                txtAuthor.Text = selectedRow.Cells("Author").Value.ToString()
                cboCategories.Text = selectedRow.Cells("Category").Value.ToString()
                txtQuantity.Text = selectedRow.Cells("Quantity").Value.ToString()
                txtPrice.Text = selectedRow.Cells("Price").Value.ToString()


                key = Convert.ToInt32(selectedRow.Cells("Book ID").Value)
            Catch ex As Exception
                MsgBox("Error: " & ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MsgBox("Please select a book to edit.", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnBDelete_Click(sender As Object, e As EventArgs) Handles btnBDelete.Click
        If dgBooksList.SelectedRows.Count = 0 Then
            MsgBox("Please select a book to delete", MessageBoxButtons.OK + MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgBooksList.SelectedRows(0)
        Dim bookID As Integer = Convert.ToInt32(selectedRow.Cells("Book ID").Value)

        Dim result As DialogResult = MsgBox("Are you sure you want to delete this book?", MessageBoxButtons.YesNo + MessageBoxIcon.Question)
        If result = DialogResult.No Then
            Return
        End If

        Try
            con.Open()
            cmd = New OleDbCommand("DELETE FROM books WHERE [Book Id] = ?", con)
            cmd.Parameters.AddWithValue("?", bookID)
            cmd.ExecuteNonQuery()
            con.Close()

            MsgBox("Book deleted successfully!", MessageBoxButtons.OK + MessageBoxIcon.Information)

            ' Reload the books list
            LoadBooksData()

        Catch ex As Exception
            MsgBox("Error: " & ex.Message, MessageBoxButtons.OK + MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        txtID.Text = ""
        txtBTitle.Text = ""
        txtAuthor.Text = ""
        cboCategories.Text = ""
        txtQuantity.Text = ""
        txtPrice.Text = ""
    End Sub
End Class