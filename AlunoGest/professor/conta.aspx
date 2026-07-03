<%@ Page Title="A minha conta" Language="C#" MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true" CodeBehind="conta.aspx.cs" Inherits="AlunoGest.professor.conta" %>
<asp:Content ID="Title" ContentPlaceHolderID="titleContent" runat="server">A minha conta</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <div class="row justify-content-center"><div class="col-lg-7">
        <div class="card page-card"><div class="card-body p-4">
            <h1 class="h3 mb-2">A minha conta</h1>
            <asp:Panel ID="PainelPrimeiroAcesso" runat="server" Visible="false" CssClass="alert alert-info">
                Este é o primeiro acesso. Por segurança, escolha um novo utilizador e uma nova palavra-passe antes de continuar.
            </asp:Panel>
            <asp:Label ID="LblMensagem" runat="server" Visible="false" />
            <div class="mb-3"><label class="form-label">Novo utilizador</label>
                <asp:TextBox ID="TxtUsername" runat="server" CssClass="form-control" MaxLength="50" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtUsername" ErrorMessage="Indique o utilizador." CssClass="text-danger small" ValidationGroup="conta" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtUsername" ValidationExpression="^[A-Za-z0-9._-]{4,50}$"
                    ErrorMessage="Use entre 4 e 50 letras, números, ponto, hífen ou underscore." CssClass="text-danger small d-block" ValidationGroup="conta" />
            </div>
            <div class="mb-3"><label class="form-label">Palavra-passe atual</label>
                <asp:TextBox ID="TxtPasswordAtual" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPasswordAtual" ErrorMessage="Indique a palavra-passe atual." CssClass="text-danger small" ValidationGroup="conta" />
            </div>
            <div class="row g-3"><div class="col-md-6"><label class="form-label">Nova palavra-passe</label>
                <asp:TextBox ID="TxtNovaPassword" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtNovaPassword" ErrorMessage="Indique a nova palavra-passe." CssClass="text-danger small" ValidationGroup="conta" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="TxtNovaPassword" ValidationExpression="^.{8,}$" ErrorMessage="Use pelo menos 8 caracteres." CssClass="text-danger small d-block" ValidationGroup="conta" />
            </div><div class="col-md-6"><label class="form-label">Confirmar palavra-passe</label>
                <asp:TextBox ID="TxtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:CompareValidator runat="server" ControlToValidate="TxtConfirmarPassword" ControlToCompare="TxtNovaPassword" ErrorMessage="As palavras-passe não coincidem." CssClass="text-danger small" ValidationGroup="conta" />
            </div></div>
            <asp:Button ID="BtnGuardar" runat="server" Text="Guardar novas credenciais" CssClass="btn btn-primary mt-4" OnClick="BtnGuardar_Click" ValidationGroup="conta" />
        </div></div>
    </div></div>
</asp:Content>
