<%@ Page Title="Calendario" Language="C#" MasterPageFile="~/professor/ModeloProfessor.Master"
    AutoEventWireup="true" CodeBehind="calendario.aspx.cs" Inherits="AlunoGest.professor.calendario" %>
<asp:Content ID="Title" ContentPlaceHolderID="titleContent" runat="server">Calendario</asp:Content>
<asp:Content ID="Head" ContentPlaceHolderID="headContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>
    <style>
        #calendar { background:#fff; border-radius:16px; min-height:620px; }
        .fc .fc-toolbar-title { font-size:1.25rem; }
    </style>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <div class="app-page-header">
        <div>
            <h1 class="app-page-title">Calendario da turma</h1>
            <p class="app-page-subtitle">Cada turma tem o seu proprio calendario. Tudo o que criar aqui fica apenas nesta turma.</p>
        </div>
    </div>

    <asp:Label ID="LblMensagem" runat="server" Visible="false" />

    <div class="card app-card mb-4">
        <div class="card-body text-start">
            <div class="row g-3 align-items-end">
                <div class="col-md-5">
                    <label class="form-label">Turma</label>
                    <asp:DropDownList ID="DdlTurmas" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="DdlTurmas_SelectedIndexChanged" />
                </div>
                <div class="col-md-7">
                    <div class="alert alert-info mb-0">Use o botao Calendario no dashboard para abrir diretamente a turma certa.</div>
                </div>
            </div>
        </div>
    </div>

    <div class="card app-card mb-4">
        <div class="card-body text-start">
            <h2 class="app-card-title">Novo/editar evento</h2>
            <asp:HiddenField ID="HdnEventoId" runat="server" />
            <div class="row g-3 align-items-end">
                <div class="col-md-2">
                    <label class="form-label">Tipo</label>
                    <asp:DropDownList ID="DdlTipo" runat="server" CssClass="form-select">
                        <asp:ListItem>Trabalho</asp:ListItem>
                        <asp:ListItem>Teste</asp:ListItem>
                        <asp:ListItem>Aviso</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Titulo</label>
                    <asp:TextBox ID="TxtTitulo" runat="server" CssClass="form-control" MaxLength="200" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Data e hora</label>
                    <asp:TextBox ID="TxtDataHora" runat="server" CssClass="form-control" TextMode="DateTimeLocal" />
                </div>
                <div class="col-md-3">
                    <label class="form-label">Anexo do professor</label>
                    <asp:FileUpload ID="FileAnexo" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="mt-3">
                <asp:Button ID="BtnGuardar" runat="server" Text="Guardar evento" CssClass="btn btn-primary" OnClick="BtnGuardar_Click" />
                <asp:Button ID="BtnLimpar" runat="server" Text="Limpar" CssClass="btn btn-outline-secondary" OnClick="BtnLimpar_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="row g-4">
        <div class="col-lg-7">
            <div class="card app-card">
                <div class="card-body text-start">
                    <div id="calendar"></div>
                </div>
            </div>
        </div>
        <div class="col-lg-5">
            <div class="card app-card mb-4">
                <div class="card-body text-start">
                    <h2 class="app-card-title">Eventos da turma</h2>
                    <asp:GridView ID="GridEventos" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0"
                        GridLines="None" DataKeyNames="Id" EmptyDataText="Ainda nao existem eventos." OnRowCommand="GridEventos_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                            <asp:BoundField DataField="Titulo" HeaderText="Titulo" />
                            <asp:BoundField DataField="DataHora" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" Text="Editar" CommandName="EditarEvento" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-outline-primary btn-sm me-1" />
                                    <asp:LinkButton runat="server" Text="Apagar" CommandName="ApagarEvento" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-outline-danger btn-sm"
                                        OnClientClick="return confirm('Apagar este evento?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="card app-card">
                <div class="card-body text-start">
                    <h2 class="app-card-title">Entregas dos alunos</h2>
                    <asp:GridView ID="GridEntregas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle mb-0"
                        GridLines="None" DataKeyNames="Id" EmptyDataText="Ainda nao existem entregas." OnRowCommand="GridEntregas_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Aluno" HeaderText="Aluno" />
                            <asp:BoundField DataField="Evento" HeaderText="Evento" />
                            <asp:TemplateField HeaderText="Ficheiro">
                                <ItemTemplate><a href='<%# Eval("CaminhoFicheiro") %>' target="_blank"><%# Eval("NomeFicheiro") %></a></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Nota" HeaderText="Nota" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" Text="Avaliar" CommandName="AvaliarEntrega" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-success btn-sm" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="PainelAvaliacao" runat="server" Visible="false" CssClass="card app-card mt-4">
        <div class="card-body text-start">
            <h2 class="app-card-title">Avaliar entrega</h2>
            <asp:HiddenField ID="HdnEntregaId" runat="server" />
            <div class="row g-3">
                <div class="col-md-2">
                    <label class="form-label">Nota</label>
                    <asp:TextBox ID="TxtNota" runat="server" CssClass="form-control" placeholder="0-20" />
                </div>
                <div class="col-md-10">
                    <label class="form-label">Feedback</label>
                    <asp:TextBox ID="TxtFeedback" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
            </div>
            <div class="mt-3">
                <asp:Button ID="BtnGuardarAvaliacao" runat="server" Text="Guardar avaliacao" CssClass="btn btn-success" OnClick="BtnGuardarAvaliacao_Click" />
                <asp:Button ID="BtnCancelarAvaliacao" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary" OnClick="BtnCancelarAvaliacao_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="HdnEvents" runat="server" />
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var el = document.getElementById('calendar');
            var calendar = new FullCalendar.Calendar(el, {
                initialView: 'dayGridMonth',
                locale: 'pt',
                headerToolbar: { left: 'prev,next today', center: 'title', right: 'dayGridMonth,timeGridWeek,timeGridDay' },
                buttonText: { today: 'Hoje', month: 'Mes', week: 'Semana', day: 'Dia' },
                events: JSON.parse(document.getElementById('<%= HdnEvents.ClientID %>').value || '[]')
            });
            calendar.render();
        });
    </script>
</asp:Content>
