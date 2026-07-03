<%@ Page Title="Dashboard do aluno" Language="C#" MasterPageFile="~/aluno/MasterAluno1.Master"
    AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="AlunoGest.aluno.dashboard" %>
<asp:Content ID="Head" ContentPlaceHolderID="headContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>
    <style>#calendar{background:#fff;border-radius:14px;padding:12px;min-height:560px}</style>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="mainContent" runat="server">
    <div class="d-flex flex-wrap justify-content-between align-items-start gap-3 mb-4">
        <div>
            <h1 class="h3 mb-1">O meu dashboard</h1>
            <asp:Label ID="LblResumo" runat="server" CssClass="text-muted" />
        </div>
    </div>
    <asp:Label ID="LblMensagem" runat="server" Visible="false" />

    <div class="row g-4">
        <div class="col-lg-7">
            <div class="card page-card">
                <div class="card-body">
                    <div id="calendar"></div>
                </div>
            </div>
        </div>
        <div class="col-lg-5">
            <div class="card page-card mb-4">
                <div class="card-body">
                    <h2 class="h5">Trabalhos e testes</h2>
                    <asp:GridView ID="GridEventos" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle"
                        GridLines="None" DataKeyNames="Id" EmptyDataText="Ainda nao existem eventos da turma." OnRowCommand="GridEventos_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                            <asp:BoundField DataField="Titulo" HeaderText="Titulo" />
                            <asp:BoundField DataField="DataHora" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="EstadoEntrega" HeaderText="Entrega" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" Text="Abrir" CommandName="AbrirEvento" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-primary btn-sm" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="card page-card">
                <div class="card-body">
                    <h2 class="h5">Notas e feedback</h2>
                    <asp:GridView ID="GridNotas" runat="server" AutoGenerateColumns="false" CssClass="table table-hover align-middle"
                        GridLines="None" EmptyDataText="Ainda nao existem avaliacoes.">
                        <Columns>
                            <asp:BoundField DataField="Evento" HeaderText="Evento" />
                            <asp:BoundField DataField="Nota" HeaderText="Nota" />
                            <asp:BoundField DataField="Feedback" HeaderText="Feedback" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="PainelEntrega" runat="server" Visible="false" CssClass="card page-card mt-4">
        <div class="card-body">
            <h2 class="h5"><asp:Label ID="LblEventoSelecionado" runat="server" /></h2>
            <asp:HiddenField ID="HdnEventoId" runat="server" />
            <p class="text-muted mb-2">Anexos publicados pelo professor:</p>
            <asp:Repeater ID="RepeaterAnexosProfessor" runat="server">
                <ItemTemplate><div><a href='<%# Eval("CaminhoFicheiro") %>' target="_blank"><%# Eval("NomeFicheiro") %></a></div></ItemTemplate>
            </asp:Repeater>
            <hr />
            <div class="row g-3">
                <div class="col-md-5">
                    <label class="form-label">Anexo da entrega</label>
                    <asp:FileUpload ID="FileEntrega" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-7">
                    <label class="form-label">Observacao</label>
                    <asp:TextBox ID="TxtObservacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>
            </div>
            <div class="mt-3">
                <asp:Button ID="BtnEntregar" runat="server" Text="Enviar entrega" CssClass="btn btn-success" OnClick="BtnEntregar_Click" />
                <asp:Button ID="BtnFecharEntrega" runat="server" Text="Fechar" CssClass="btn btn-outline-secondary" OnClick="BtnFecharEntrega_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="HdnEvents" runat="server" />
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var calendar = new FullCalendar.Calendar(document.getElementById('calendar'), {
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
