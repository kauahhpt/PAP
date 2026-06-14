<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="AlunoGest.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inovar inovando</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="d-flex justify-content-center align-items-center" style="height: 100vh;">
            <div class="p-4 bg-light border rounded">
                <asp:Login ID="loginUtilizador" runat="server"
                    CreateUserText="Criar conta"
                    CreateUserUrl="criarConta.aspx"
                    OnLoggedIn="loginUtilizador_LoggedIn"></asp:Login>
            </div>
        </div>
    </form>
</body>
</html>
