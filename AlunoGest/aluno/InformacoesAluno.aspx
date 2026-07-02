<%@ Page Title="" Language="C#" MasterPageFile="~/aluno/MasterAluno1.Master" AutoEventWireup="true" CodeBehind="InformacoesAluno.aspx.cs" Inherits="AlunoGest.aluno.InformacoesAluno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
    <style>
        .perfil-card {
            max-width: 850px;
            margin: auto;
            background: #fff;
            padding: 25px;
            border-radius: 10px;
        }

        .foto-perfil {
            width: 150px;
            height: 150px;
            object-fit: cover;
            border-radius: 50%;
            border: 3px solid #55CCCC;
        }

        .info-label {
            font-weight: bold;
            color: #555;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="perfil-card">
        <h2>As Minhas Informações</h2>

        <asp:Label ID="LblMensagem" runat="server" Visible="false"></asp:Label>

        <div class="text-center mb-4">
            <asp:Image ID="ImgFotoPerfil" runat="server" CssClass="foto-perfil" />
        </div>

        <div class="mb-3">
            <span class="info-label">Nome completo:</span>
            <asp:Label ID="LblNomeCompleto" runat="server"></asp:Label>
        </div>

        <div class="mb-3">
            <span class="info-label">Número de processo:</span>
            <asp:Label ID="LblNumeroProcesso" runat="server"></asp:Label>
        </div>

        <div class="mb-3">
            <span class="info-label">Username:</span>
            <asp:Label ID="LblUsername" runat="server"></asp:Label>
        </div>

        <hr />

        <h4>Alterar Foto de Perfil</h4>

        <div class="mb-3">
            <asp:FileUpload ID="FileFoto" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="BtnAlterarFoto" runat="server" Text="Alterar Foto" CssClass="btn btn-primary" OnClick="BtnAlterarFoto_Click" />

        <hr />

        <h4>Alterar Senha</h4>

        <div class="mb-3">
            <label>Senha atual</label>
            <asp:TextBox ID="TxtSenhaAtual" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label>Nova senha</label>
            <asp:TextBox ID="TxtNovaSenha" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label>Confirmar nova senha</label>
            <asp:TextBox ID="TxtConfirmarSenha" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
        </div>

        <asp:Button ID="BtnAlterarSenha" runat="server" Text="Alterar Senha" CssClass="btn btn-warning" OnClick="BtnAlterarSenha_Click" />
    </div>

</asp:Content>