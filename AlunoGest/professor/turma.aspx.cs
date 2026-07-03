using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace AlunoGest.professor
{
    public partial class turma : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int TurmaId { get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (TurmaId == 0 || !ProfessorTemTurma()) { Response.Redirect("~/professor/dashboard.aspx"); return; }
                if (!IsPostBack) { CarregarCabecalho(); CarregarAlunosDisponiveis(); CarregarAlunos(); CarregarAtividades(); }
            }
            catch (Exception ex) { MostrarMensagem("Não foi possível carregar a turma: " + ex.Message, true); }
        }

        protected void TxtPesquisaAluno_TextChanged(object sender, EventArgs e) { CarregarAlunos(); }
        protected void BtnAdicionarAluno_Click(object sender, EventArgs e)
        {
            int alunoId;
            if (!int.TryParse(DdlAlunosDisponiveis.SelectedValue, out alunoId)) { MostrarMensagem("Selecione um aluno disponível.", true); return; }
            if (!ProfessorTemTurma()) return;
            const string sql = @"
                INSERT INTO dbo.AlunoTurma(AlunoId,TurmaId,Desde,Ate,TemPortugues,TemEMRC)
                SELECT @AlunoId,@TurmaId,CAST(GETDATE() AS date),NULL,0,0
                WHERE EXISTS(SELECT 1 FROM dbo.Aluno a INNER JOIN dbo.Turma t ON t.Id=@TurmaId INNER JOIN dbo.Escola e ON e.Id=t.EscolaId
                             WHERE a.Id=@AlunoId AND a.AgrupamentoId=e.AgrupamentoId AND a.Ativo=1)
                  AND NOT EXISTS(SELECT 1 FROM dbo.AlunoTurma WHERE AlunoId=@AlunoId AND Ate IS NULL);";
            using (SqlConnection c=new SqlConnection(_connectionString)) using (SqlCommand cmd=new SqlCommand(sql,c))
            { cmd.Parameters.AddWithValue("@AlunoId",alunoId); cmd.Parameters.AddWithValue("@TurmaId",TurmaId); c.Open();
              if(cmd.ExecuteNonQuery()==0){MostrarMensagem("O aluno já pertence a uma turma ou deixou de estar disponível.",true);return;} }
            CarregarAlunosDisponiveis(); CarregarAlunos(); MostrarMensagem("Aluno adicionado à turma.",false);
        }

        protected void GridAlunos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName!="RemoverAluno" || !ProfessorTemTurma()) return;
            int index; if(!int.TryParse(e.CommandArgument.ToString(),out index) || index<0 || index>=GridAlunos.Rows.Count)return;
            int alunoTurmaId=Convert.ToInt32(GridAlunos.DataKeys[index].Value);
            using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("UPDATE dbo.AlunoTurma SET Ate=CAST(GETDATE() AS date) WHERE Id=@Id AND TurmaId=@TurmaId AND Ate IS NULL",c))
            {cmd.Parameters.AddWithValue("@Id",alunoTurmaId);cmd.Parameters.AddWithValue("@TurmaId",TurmaId);c.Open();cmd.ExecuteNonQuery();}
            CarregarAlunosDisponiveis(); CarregarAlunos(); MostrarMensagem("Aluno removido da turma.",false);
        }
        protected void BtnNova_Click(object sender, EventArgs e) { LimparFormulario(); PainelFormulario.Visible=true; }
        protected void BtnCancelar_Click(object sender, EventArgs e) { LimparFormulario(); PainelFormulario.Visible=false; }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !ProfessorTemTurma()) return;
            DateTime dataHora;
            if (!DateTime.TryParse(TxtDataHora.Text, out dataHora)) { MostrarMensagem("A data e hora não são válidas.", true); return; }
            try
            {
                int eventoId;
                if (string.IsNullOrEmpty(HdnEventoId.Value)) eventoId=InserirEvento(dataHora);
                else { eventoId=Convert.ToInt32(HdnEventoId.Value); AtualizarEvento(eventoId, dataHora); }
                if (FileAnexo.HasFile) GuardarAnexo(eventoId);
                LimparFormulario(); PainelFormulario.Visible=false; CarregarAtividades();
                MostrarMensagem("Atividade guardada e publicada para a turma.", false);
            }
            catch (Exception ex) { MostrarMensagem("Não foi possível guardar: "+ex.Message, true); }
        }

        protected void GridAtividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index; if (!int.TryParse(e.CommandArgument.ToString(), out index) || index<0 || index>=GridAtividades.Rows.Count) return;
            int eventoId=Convert.ToInt32(GridAtividades.DataKeys[index].Value);
            if (e.CommandName=="EditarAtividade") CarregarEvento(eventoId);
            else if (e.CommandName=="ApagarAtividade")
            {
                try { ApagarEvento(eventoId); CarregarAtividades(); PainelFormulario.Visible=false; MostrarMensagem("Atividade eliminada.", false); }
                catch (Exception ex) { MostrarMensagem("Não foi possível eliminar: "+ex.Message, true); }
            }
        }

        protected void RepeaterAnexos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName!="Remover") return;
            int eventoId; if (!int.TryParse(HdnEventoId.Value, out eventoId) || !EventoPertenceATurma(eventoId)) return;
            ApagarAnexo(Convert.ToInt32(e.CommandArgument), eventoId); CarregarAnexos(eventoId);
            MostrarMensagem("Anexo removido.", false);
        }

        private int ProfessorId
        {
            get
            {
                int id; if (Session["ProfessorID"]!=null && int.TryParse(Session["ProfessorID"].ToString(),out id)) return id;
                if (Session["UserId"]==null) throw new InvalidOperationException("A sessão terminou.");
                using (SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand("SELECT Id FROM dbo.Professor WHERE UserId=@UserId AND Ativo=1",c))
                { cmd.Parameters.AddWithValue("@UserId",new Guid(Session["UserId"].ToString())); c.Open(); object v=cmd.ExecuteScalar(); if(v==null) throw new InvalidOperationException("Professor não encontrado."); id=Convert.ToInt32(v); Session["ProfessorID"]=id; return id; }
            }
        }

        private bool ProfessorTemTurma()
        {
            const string sql=@"SELECT COUNT(1) FROM dbo.TurmaDisciplinaProfessor tdp INNER JOIN dbo.TurmaDisciplina td ON td.Id=tdp.TurmaDisciplinaId WHERE tdp.ProfessorId=@ProfessorId AND td.TurmaId=@TurmaId AND tdp.Ate IS NULL";
            using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) { cmd.Parameters.AddWithValue("@ProfessorId",ProfessorId); cmd.Parameters.AddWithValue("@TurmaId",TurmaId); c.Open(); return Convert.ToInt32(cmd.ExecuteScalar())>0; }
        }

        private void CarregarCabecalho()
        {
            const string sql=@"SELECT CAST(t.AnoEscolaridade AS varchar(2))+'.º'+t.CodigoTurma Turma,e.Nome,al.Descricao FROM dbo.Turma t INNER JOIN dbo.Escola e ON e.Id=t.EscolaId INNER JOIN dbo.AnoLetivo al ON al.Id=t.AnoLetivoId WHERE t.Id=@Id;
                SELECT STUFF((SELECT DISTINCT ', '+d.Nome FROM dbo.TurmaDisciplina td INNER JOIN dbo.TurmaDisciplinaProfessor tdp ON tdp.TurmaDisciplinaId=td.Id AND tdp.ProfessorId=@ProfessorId AND tdp.Ate IS NULL INNER JOIN dbo.Disciplina d ON d.Id=td.DisciplinaId WHERE td.TurmaId=@Id FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,2,'');";
            using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) { cmd.Parameters.AddWithValue("@Id",TurmaId); cmd.Parameters.AddWithValue("@ProfessorId",ProfessorId); c.Open(); using(SqlDataReader r=cmd.ExecuteReader()) { if(r.Read()){LblTurma.Text="Turma "+r["Turma"]; LblDetalhes.Text=r["Nome"]+" | "+r["Descricao"];} if(r.NextResult()&&r.Read()) LblDisciplinas.Text=r[0].ToString(); } }
        }

        private void CarregarAlunos()
        {
            const string sql=@"SELECT at2.Id AlunoTurmaId,ROW_NUMBER() OVER(ORDER BY a.NomeCompleto) Numero,a.NomeCompleto,a.NumeroProcesso,a.Email,at2.Desde FROM dbo.AlunoTurma at2 INNER JOIN dbo.Aluno a ON a.Id=at2.AlunoId WHERE at2.TurmaId=@TurmaId AND at2.Ate IS NULL AND (@Pesquisa='' OR a.NomeCompleto LIKE '%'+@Pesquisa+'%' OR a.NumeroProcesso LIKE '%'+@Pesquisa+'%') ORDER BY a.NomeCompleto";
            DataTable dt=new DataTable(); using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) using(SqlDataAdapter da=new SqlDataAdapter(cmd)) { cmd.Parameters.AddWithValue("@TurmaId",TurmaId); cmd.Parameters.AddWithValue("@Pesquisa",TxtPesquisaAluno.Text.Trim()); da.Fill(dt); } GridAlunos.DataSource=dt; GridAlunos.DataBind();
        }

        private void CarregarAlunosDisponiveis()
        {
            const string sql=@"SELECT a.Id,a.NomeCompleto+' ('+ISNULL(a.NumeroProcesso,'sem processo')+')' Nome FROM dbo.Aluno a INNER JOIN dbo.Turma t ON t.Id=@TurmaId INNER JOIN dbo.Escola e ON e.Id=t.EscolaId WHERE a.AgrupamentoId=e.AgrupamentoId AND a.Ativo=1 AND NOT EXISTS(SELECT 1 FROM dbo.AlunoTurma at2 WHERE at2.AlunoId=a.Id AND at2.Ate IS NULL) ORDER BY a.NomeCompleto";
            DataTable dt=new DataTable();using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand(sql,c))using(SqlDataAdapter da=new SqlDataAdapter(cmd)){cmd.Parameters.AddWithValue("@TurmaId",TurmaId);da.Fill(dt);}DdlAlunosDisponiveis.DataSource=dt;DdlAlunosDisponiveis.DataTextField="Nome";DdlAlunosDisponiveis.DataValueField="Id";DdlAlunosDisponiveis.DataBind();DdlAlunosDisponiveis.Items.Insert(0,new ListItem("-- selecionar --",""));
        }

        private void CarregarAtividades()
        {
            const string sql=@"SELECT ev.Id,ev.Tipo,ev.Titulo,ev.DataHora,(SELECT COUNT(*) FROM dbo.EventoAnexo ea WHERE ea.EventoId=ev.Id) TotalAnexos FROM dbo.Evento ev WHERE ev.TurmaId=@TurmaId ORDER BY ev.DataHora DESC";
            DataTable dt=new DataTable(); using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) using(SqlDataAdapter da=new SqlDataAdapter(cmd)) {cmd.Parameters.AddWithValue("@TurmaId",TurmaId); da.Fill(dt);} GridAtividades.DataSource=dt; GridAtividades.DataBind();
        }

        private int InserirEvento(DateTime dataHora)
        {
            const string sql=@"INSERT INTO dbo.Evento(AlunoId,TurmaId,Titulo,Tipo,DataHora) VALUES(NULL,@TurmaId,@Titulo,@Tipo,@DataHora); SELECT CAST(SCOPE_IDENTITY() AS int);";
            using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) { ParametrosEvento(cmd,dataHora); c.Open(); return Convert.ToInt32(cmd.ExecuteScalar()); }
        }
        private void AtualizarEvento(int id,DateTime dataHora)
        {
            const string sql=@"UPDATE dbo.Evento SET Titulo=@Titulo,Tipo=@Tipo,DataHora=@DataHora WHERE Id=@Id AND TurmaId=@TurmaId";
            using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) { ParametrosEvento(cmd,dataHora); cmd.Parameters.AddWithValue("@Id",id); c.Open(); if(cmd.ExecuteNonQuery()==0) throw new InvalidOperationException("Atividade não encontrada."); }
        }
        private void ParametrosEvento(SqlCommand cmd,DateTime dataHora) { cmd.Parameters.AddWithValue("@TurmaId",TurmaId); cmd.Parameters.AddWithValue("@Titulo",TxtTitulo.Text.Trim()); cmd.Parameters.AddWithValue("@Tipo",DdlTipo.SelectedValue); cmd.Parameters.AddWithValue("@DataHora",dataHora); }

        private void CarregarEvento(int id)
        {
            const string sql="SELECT Id,Titulo,Tipo,DataHora FROM dbo.Evento WHERE Id=@Id AND TurmaId=@TurmaId";
            using(SqlConnection c=new SqlConnection(_connectionString)) using(SqlCommand cmd=new SqlCommand(sql,c)) {cmd.Parameters.AddWithValue("@Id",id);cmd.Parameters.AddWithValue("@TurmaId",TurmaId);c.Open();using(SqlDataReader r=cmd.ExecuteReader()){if(!r.Read())return;HdnEventoId.Value=r["Id"].ToString();TxtTitulo.Text=r["Titulo"].ToString();DdlTipo.SelectedValue=r["Tipo"].ToString();TxtDataHora.Text=Convert.ToDateTime(r["DataHora"]).ToString("yyyy-MM-ddTHH:mm");}}
            PainelFormulario.Visible=true; CarregarAnexos(id);
        }
        private bool EventoPertenceATurma(int id) { using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("SELECT COUNT(1) FROM dbo.Evento WHERE Id=@Id AND TurmaId=@TurmaId",c)){cmd.Parameters.AddWithValue("@Id",id);cmd.Parameters.AddWithValue("@TurmaId",TurmaId);c.Open();return Convert.ToInt32(cmd.ExecuteScalar())>0;} }

        private void GuardarAnexo(int eventoId)
        {
            if(FileAnexo.PostedFile.ContentLength>10*1024*1024) throw new InvalidOperationException("O anexo excede 10 MB.");
            string original=Path.GetFileName(FileAnexo.FileName); string nome=Guid.NewGuid().ToString("N")+"_"+original; string pasta=Server.MapPath("~/uploads/eventos/"); Directory.CreateDirectory(pasta); FileAnexo.SaveAs(Path.Combine(pasta,nome));
            using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("INSERT INTO dbo.EventoAnexo(EventoId,NomeFicheiro,CaminhoFicheiro) VALUES(@EventoId,@Nome,@Caminho)",c)){cmd.Parameters.AddWithValue("@EventoId",eventoId);cmd.Parameters.AddWithValue("@Nome",original);cmd.Parameters.AddWithValue("@Caminho",ResolveUrl("~/uploads/eventos/"+nome));c.Open();cmd.ExecuteNonQuery();}
        }
        private void CarregarAnexos(int eventoId) { DataTable dt=new DataTable();using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("SELECT Id,NomeFicheiro,CaminhoFicheiro FROM dbo.EventoAnexo WHERE EventoId=@Id ORDER BY CreatedAt",c))using(SqlDataAdapter da=new SqlDataAdapter(cmd)){cmd.Parameters.AddWithValue("@Id",eventoId);da.Fill(dt);}RepeaterAnexos.DataSource=dt;RepeaterAnexos.DataBind(); }
        private void ApagarAnexo(int id,int eventoId)
        {
            string caminho=null;using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("SELECT CaminhoFicheiro FROM dbo.EventoAnexo WHERE Id=@Id AND EventoId=@EventoId",c)){cmd.Parameters.AddWithValue("@Id",id);cmd.Parameters.AddWithValue("@EventoId",eventoId);c.Open();object v=cmd.ExecuteScalar();if(v!=null)caminho=v.ToString();}
            if(caminho==null)return;using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("DELETE FROM dbo.EventoAnexo WHERE Id=@Id AND EventoId=@EventoId",c)){cmd.Parameters.AddWithValue("@Id",id);cmd.Parameters.AddWithValue("@EventoId",eventoId);c.Open();cmd.ExecuteNonQuery();} ApagarFicheiro(caminho);
        }
        private void ApagarEvento(int id)
        {
            if(!EventoPertenceATurma(id))return;List<string> caminhos=new List<string>();using(SqlConnection c=new SqlConnection(_connectionString))using(SqlCommand cmd=new SqlCommand("SELECT CaminhoFicheiro FROM dbo.EventoAnexo WHERE EventoId=@Id",c)){cmd.Parameters.AddWithValue("@Id",id);c.Open();using(SqlDataReader r=cmd.ExecuteReader())while(r.Read())caminhos.Add(r[0].ToString());}
            using(SqlConnection c=new SqlConnection(_connectionString)){c.Open();SqlTransaction t=c.BeginTransaction();try{using(SqlCommand a=new SqlCommand("DELETE FROM dbo.EventoAnexo WHERE EventoId=@Id",c,t)){a.Parameters.AddWithValue("@Id",id);a.ExecuteNonQuery();}using(SqlCommand e=new SqlCommand("DELETE FROM dbo.Evento WHERE Id=@Id AND TurmaId=@TurmaId",c,t)){e.Parameters.AddWithValue("@Id",id);e.Parameters.AddWithValue("@TurmaId",TurmaId);e.ExecuteNonQuery();}t.Commit();}catch{t.Rollback();throw;}} foreach(string p in caminhos)ApagarFicheiro(p);
        }
        private void ApagarFicheiro(string caminho) { try { string p=Server.MapPath(caminho);if(File.Exists(p))File.Delete(p); } catch { } }
        private void LimparFormulario(){HdnEventoId.Value="";TxtTitulo.Text="";TxtDataHora.Text="";DdlTipo.SelectedIndex=0;RepeaterAnexos.DataSource=null;RepeaterAnexos.DataBind();}
        private void MostrarMensagem(string texto,bool erro){LblMensagem.Text=texto;LblMensagem.CssClass=erro?"alert alert-warning d-block":"alert alert-success d-block";LblMensagem.Visible=true;}
    }
}
