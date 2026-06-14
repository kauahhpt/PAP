<%@ Page Title="" Language="C#" MasterPageFile="~/aluno/MasterAluno1.Master" AutoEventWireup="true" CodeBehind="HomeAluno.aspx.cs" Inherits="AlunoGest.aluno.HomeAluno" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">

    <!-- FullCalendar CSS -->
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css" rel="stylesheet" />

    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <!-- FullCalendar JS -->
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>

    <style>
        body {
            font-family: Arial;
        }

        h2 {
            color: #55CCCC;
        }

        .container {
            background-color: #fff;
            padding: 20px;
            margin: 20px auto;
            border-radius: 8px;
            max-width: 900px;
        }

        #calendar {
            margin-top: 20px;
        }

        .form-linha {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            align-items: end;
            margin-bottom: 10px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div class="container">
        <h2>A Minha Agenda</h2>

        <asp:Label ID="LblMensagem" runat="server" Visible="false"></asp:Label>

        <div class="form-linha">

            <div>
                <label class="form-label d-block">Tipo</label>
                <asp:DropDownList ID="DdlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Teste" Value="Teste"></asp:ListItem>
                    <asp:ListItem Text="Trabalho" Value="Trabalho"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div style="flex: 1; min-width: 200px;">
                <label class="form-label d-block">Título</label>
                <asp:TextBox ID="TxtTitulo" runat="server" CssClass="form-control" placeholder="Título"></asp:TextBox>
            </div>

            <div>
                <label class="form-label d-block">Data e Hora</label>
                <asp:TextBox ID="TxtDataHora" runat="server" CssClass="form-control" TextMode="DateTimeLocal"></asp:TextBox>
            </div>

            <div>
                <label class="form-label d-block">Anexo</label>
                <asp:FileUpload ID="FileAnexo" runat="server" CssClass="form-control" />
            </div>

        </div>

        <asp:HiddenField ID="HdnEventoId" runat="server" />

        <div class="mb-3">
            <asp:Button ID="ButtonAdicionar" runat="server" Text="Adicionar" CssClass="btn btn-primary" OnClick="ButtonAdicionar_Click" />
            <asp:Button ID="ButtonAtualizar" runat="server" Text="Atualizar" CssClass="btn btn-secondary" OnClick="ButtonAtualizar_Click" />
            <asp:Button ID="ButtonApagar" runat="server" Text="Apagar" CssClass="btn btn-outline-danger" OnClick="ButtonApagar_Click" CausesValidation="false" />
            <asp:Button ID="ButtonLimpar" runat="server" Text="Limpar" CssClass="btn btn-outline-secondary" OnClick="ButtonLimpar_Click" CausesValidation="false" />
        </div>

        <!-- Anexos do evento seleccionado -->
        <div id="PainelAnexos" runat="server" visible="false" class="mb-3">
            <h5>Anexos</h5>
            <asp:Repeater ID="RepeaterAnexos" runat="server">
                <ItemTemplate>
                    <div>
                        <a href='<%# Eval("CaminhoFicheiro") %>' target="_blank"><%# Eval("NomeFicheiro") %></a>
                        <asp:LinkButton runat="server"
                            Text="Remover"
                            CommandName="RemoverAnexo"
                            CommandArgument='<%# Eval("Id") %>'
                            OnCommand="LinkButtonRemoverAnexo_Command"
                            CssClass="text-danger ms-2" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div id="calendar"></div>
        <asp:HiddenField ID="HdnEvents" runat="server" />
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {

            var calendarEl = document.getElementById('calendar');

            var calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'timeGridWeek',
                locale: 'pt',
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay'
                },
                buttonText: {
                    today: 'Hoje',
                    month: 'Mensal',
                    week: 'Semanal',
                    day: 'Diária'
                },
                events: JSON.parse(document.getElementById('<%= HdnEvents.ClientID %>').value || '[]'),
                eventTimeFormat: { hour: '2-digit', minute: '2-digit', meridiem: false },
                eventClick: function (info) {
                    if (info.event.extendedProps.tipoOrigem !== 'Pessoal') {
                        // Eventos de turma não são editáveis pelo aluno.
                        alert('Este evento foi criado pelo professor da turma.');
                        return;
                    }

                    document.getElementById('<%= TxtTitulo.ClientID %>').value = info.event.title;
                    document.getElementById('<%= TxtDataHora.ClientID %>').value = info.event.start.toISOString().slice(0, 16);
                    document.getElementById('<%= HdnEventoId.ClientID %>').value = info.event.id;

                    var ddlTipo = document.getElementById('<%= DdlTipo.ClientID %>');
                    ddlTipo.value = info.event.extendedProps.tipo;

                    // Submete para o servidor mostrar os anexos do evento seleccionado.
                    __doPostBack('<%= ButtonAtualizar.UniqueID %>', 'mostrarAnexos');
                }
            });

            calendar.render();
        });
    </script>

</asp:Content>