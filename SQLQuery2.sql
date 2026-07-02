USE [master]
GO
/****** Object:  Database [GestaoEscolar]    Script Date: 21/06/2026 20:46:13 ******/
CREATE DATABASE [GestaoEscolar]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'GestaoEscolar', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\GestaoEscolar.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'GestaoEscolar_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\GestaoEscolar_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [GestaoEscolar] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GestaoEscolar].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GestaoEscolar] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GestaoEscolar] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GestaoEscolar] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GestaoEscolar] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GestaoEscolar] SET ARITHABORT OFF 
GO
ALTER DATABASE [GestaoEscolar] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [GestaoEscolar] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GestaoEscolar] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GestaoEscolar] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GestaoEscolar] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GestaoEscolar] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GestaoEscolar] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GestaoEscolar] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GestaoEscolar] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GestaoEscolar] SET  ENABLE_BROKER 
GO
ALTER DATABASE [GestaoEscolar] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GestaoEscolar] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GestaoEscolar] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GestaoEscolar] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GestaoEscolar] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GestaoEscolar] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GestaoEscolar] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GestaoEscolar] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [GestaoEscolar] SET  MULTI_USER 
GO
ALTER DATABASE [GestaoEscolar] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GestaoEscolar] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GestaoEscolar] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GestaoEscolar] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [GestaoEscolar] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [GestaoEscolar] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [GestaoEscolar] SET QUERY_STORE = ON
GO
ALTER DATABASE [GestaoEscolar] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [GestaoEscolar]
GO
/****** Object:  Table [dbo].[Agrupamento]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agrupamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[CodigoMEC] [nvarchar](20) NULL,
	[Morada] [nvarchar](300) NULL,
	[CodigoPostal] [char](8) NULL,
	[Localidade] [nvarchar](100) NULL,
	[Email] [nvarchar](150) NULL,
	[Telefone] [nvarchar](30) NULL,
	[UserId] [uniqueidentifier] NULL,
	[Ativo] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Aluno]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Aluno](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgrupamentoId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[NomeCompleto] [nvarchar](200) NOT NULL,
	[NumeroProcesso] [nvarchar](50) NULL,
	[Email] [nvarchar](150) NULL,
	[Ativo] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AlunoTurma]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlunoTurma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AlunoId] [int] NOT NULL,
	[TurmaId] [int] NOT NULL,
	[Desde] [date] NOT NULL,
	[Ate] [date] NULL,
	[TemPortugues] [bit] NOT NULL,
	[TemEMRC] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnoLetivo]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnoLetivo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descricao] [nvarchar](20) NOT NULL,
	[DataInicio] [date] NULL,
	[DataFim] [date] NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AnoLetivo_Descricao] UNIQUE NONCLUSTERED 
(
	[Descricao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applications](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[ApplicationName] [nvarchar](235) NOT NULL,
	[Description] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Disciplina]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Disciplina](
	[Id] [int] NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[GrupoDisciplinarId] [int] NOT NULL,
	[Ativa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Disciplina_Nome] UNIQUE NONCLUSTERED 
(
	[Nome] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Escola]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Escola](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgrupamentoId] [int] NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[CodigoMEC] [nvarchar](20) NULL,
	[Morada] [nvarchar](300) NULL,
	[CodigoPostal] [varchar](8) NULL,
	[Localidade] [nvarchar](100) NULL,
	[Email] [nvarchar](150) NULL,
	[Telefone] [nvarchar](30) NULL,
	[Ativa] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Evento]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Evento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AlunoId] [int] NULL,
	[TurmaId] [int] NULL,
	[Titulo] [nvarchar](200) NOT NULL,
	[Tipo] [nvarchar](20) NOT NULL,
	[DataHora] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventoAnexo]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventoAnexo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventoId] [int] NOT NULL,
	[NomeFicheiro] [nvarchar](255) NOT NULL,
	[CaminhoFicheiro] [nvarchar](500) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GrupoDisciplinar]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GrupoDisciplinar](
	[Id] [int] NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[CorHex] [nvarchar](7) NOT NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_GrupoDisciplinar_Nome] UNIQUE NONCLUSTERED 
(
	[Nome] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GrupoRecrutamento]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GrupoRecrutamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](10) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_GrupoRecrutamento_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HorarioTurma]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HorarioTurma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurmaDisciplinaId] [int] NOT NULL,
	[SalaId] [int] NOT NULL,
	[DiaSemana] [tinyint] NOT NULL,
	[HoraInicio] [time](7) NOT NULL,
	[HoraFim] [time](7) NOT NULL,
	[Desde] [date] NOT NULL,
	[Ate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Memberships]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Memberships](
	[UserId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordFormat] [int] NOT NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[PasswordQuestion] [nvarchar](256) NULL,
	[PasswordAnswer] [nvarchar](128) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[FailedPasswordAttemptCount] [int] NOT NULL,
	[FailedPasswordAttemptWindowStart] [datetime] NOT NULL,
	[FailedPasswordAnswerAttemptCount] [int] NOT NULL,
	[FailedPasswordAnswerAttemptWindowsStart] [datetime] NOT NULL,
	[Comment] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfertaEscolaAgrupamento]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfertaEscolaAgrupamento](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgrupamentoId] [int] NOT NULL,
	[AnoLetivoId] [int] NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[Descricao] [nvarchar](500) NULL,
	[Ativa] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfertaEscolaAgrupamentoId] [int] NOT NULL,
	[GrupoDisciplinarId] [int] NOT NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_OfertaEscolaAgrupamentoGrupo] UNIQUE NONCLUSTERED 
(
	[OfertaEscolaAgrupamentoId] ASC,
	[GrupoDisciplinarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfertaFormativa]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfertaFormativa](
	[Id] [int] NOT NULL,
	[Codigo] [nvarchar](10) NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[NivelEnsino] [nvarchar](30) NOT NULL,
	[Ativa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_OfertaFormativa_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanoCurricular]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanoCurricular](
	[Id] [int] NOT NULL,
	[OfertaFormativaId] [int] NOT NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[DataInicioVigencia] [date] NOT NULL,
	[DataFimVigencia] [date] NULL,
	[Ativo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanoCurricularDisciplina]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanoCurricularDisciplina](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanoCurricularId] [int] NOT NULL,
	[DisciplinaId] [int] NOT NULL,
	[AnoEscolaridade] [tinyint] NOT NULL,
	[Componente] [nvarchar](50) NOT NULL,
	[Natureza] [nvarchar](20) NOT NULL,
	[TipoDuracao] [nvarchar](20) NOT NULL,
	[BlocoOpcao] [nvarchar](20) NULL,
	[Observacoes] [nvarchar](500) NULL,
	[Ativa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Professor]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Professor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AgrupamentoId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Nome] [nvarchar](200) NOT NULL,
	[NumeroProcesso] [nvarchar](50) NULL,
	[GrupoRecrutamentoId] [int] NULL,
	[Email] [nvarchar](150) NULL,
	[Ativo] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProfessorDisciplina]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfessorDisciplina](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfessorId] [int] NOT NULL,
	[DisciplinaId] [int] NOT NULL,
	[Desde] [date] NULL,
	[Ate] [date] NULL,
	[Observacoes] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profiles]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profiles](
	[UserId] [uniqueidentifier] NOT NULL,
	[PropertyNames] [nvarchar](max) NOT NULL,
	[PropertyValueStrings] [nvarchar](max) NOT NULL,
	[PropertyValueBinary] [varbinary](max) NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[Description] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sala]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sala](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EscolaId] [int] NOT NULL,
	[Nome] [nvarchar](50) NOT NULL,
	[Capacidade] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Turma]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Turma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EscolaId] [int] NOT NULL,
	[AnoLetivoId] [int] NOT NULL,
	[PlanoCurricularId] [int] NOT NULL,
	[AnoEscolaridade] [tinyint] NOT NULL,
	[CodigoTurma] [nvarchar](20) NOT NULL,
	[Ativa] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TurmaDiretor]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TurmaDiretor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurmaId] [int] NOT NULL,
	[ProfessorId] [int] NOT NULL,
	[Desde] [date] NOT NULL,
	[Ate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TurmaDisciplina]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TurmaDisciplina](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurmaId] [int] NOT NULL,
	[DisciplinaId] [int] NOT NULL,
	[PlanoCurricularDisciplinaId] [int] NOT NULL,
	[OfertaEscolaAgrupamentoId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TurmaDisciplinaProfessor]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TurmaDisciplinaProfessor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurmaDisciplinaId] [int] NOT NULL,
	[ProfessorId] [int] NOT NULL,
	[Desde] [date] NOT NULL,
	[Ate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[IsAnonymous] [bit] NOT NULL,
	[LastActivityDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersInRoles]    Script Date: 21/06/2026 20:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersInRoles](
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UsersInRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_OfertaEscolaAgrupamento_AgrupamentoAno]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IX_OfertaEscolaAgrupamento_AgrupamentoAno] ON [dbo].[OfertaEscolaAgrupamento]
(
	[AgrupamentoId] ASC,
	[AnoLetivoId] ASC,
	[Ativa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProfessorDisciplina_Disciplina]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IX_ProfessorDisciplina_Disciplina] ON [dbo].[ProfessorDisciplina]
(
	[DisciplinaId] ASC,
	[Ate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Turma_Escola_AnoLetivo]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IX_Turma_Escola_AnoLetivo] ON [dbo].[Turma]
(
	[EscolaId] ASC,
	[AnoLetivoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TurmaDisciplina_Turma]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IX_TurmaDisciplina_Turma] ON [dbo].[TurmaDisciplina]
(
	[TurmaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TurmaDisciplinaProfessor_TurmaDisciplina]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IX_TurmaDisciplinaProfessor_TurmaDisciplina] ON [dbo].[TurmaDisciplinaProfessor]
(
	[TurmaDisciplinaId] ASC,
	[Ate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_UserName]    Script Date: 21/06/2026 20:46:13 ******/
CREATE NONCLUSTERED INDEX [IDX_UserName] ON [dbo].[Users]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Agrupamento] ADD  CONSTRAINT [DF_Agrupamento_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[Agrupamento] ADD  CONSTRAINT [DF_Agrupamento_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Aluno] ADD  CONSTRAINT [DF_Aluno_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[Aluno] ADD  CONSTRAINT [DF_Aluno_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AlunoTurma] ADD  DEFAULT ((1)) FOR [TemPortugues]
GO
ALTER TABLE [dbo].[AlunoTurma] ADD  DEFAULT ((0)) FOR [TemEMRC]
GO
ALTER TABLE [dbo].[AnoLetivo] ADD  CONSTRAINT [DF_AnoLetivo_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[Disciplina] ADD  CONSTRAINT [DF_Disciplina_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[Escola] ADD  CONSTRAINT [DF_Escola_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[Escola] ADD  CONSTRAINT [DF_Escola_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Evento] ADD  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[EventoAnexo] ADD  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[GrupoDisciplinar] ADD  CONSTRAINT [DF_GrupoDisciplinar_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[GrupoRecrutamento] ADD  CONSTRAINT [DF_GrupoRecrutamento_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento] ADD  CONSTRAINT [DF_OfertaEscolaAgrupamento_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento] ADD  CONSTRAINT [DF_OfertaEscolaAgrupamento_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar] ADD  CONSTRAINT [DF_OfertaEscolaAgrupamentoGrupoDisciplinar_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[OfertaFormativa] ADD  CONSTRAINT [DF_OfertaFormativa_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[PlanoCurricular] ADD  CONSTRAINT [DF_PlanoCurricular_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[PlanoCurricularDisciplina] ADD  CONSTRAINT [DF_PlanoCurricularDisciplina_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[Professor] ADD  CONSTRAINT [DF_Professor_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[Professor] ADD  CONSTRAINT [DF_Professor_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Turma] ADD  CONSTRAINT [DF_Turma_Ativa]  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[Turma] ADD  CONSTRAINT [DF_Turma_CreatedAt]  DEFAULT (sysdatetime()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Aluno]  WITH CHECK ADD  CONSTRAINT [FK_Aluno_Agrupamento] FOREIGN KEY([AgrupamentoId])
REFERENCES [dbo].[Agrupamento] ([Id])
GO
ALTER TABLE [dbo].[Aluno] CHECK CONSTRAINT [FK_Aluno_Agrupamento]
GO
ALTER TABLE [dbo].[AlunoTurma]  WITH CHECK ADD  CONSTRAINT [FK_AlunoTurma_Aluno] FOREIGN KEY([AlunoId])
REFERENCES [dbo].[Aluno] ([Id])
GO
ALTER TABLE [dbo].[AlunoTurma] CHECK CONSTRAINT [FK_AlunoTurma_Aluno]
GO
ALTER TABLE [dbo].[AlunoTurma]  WITH CHECK ADD  CONSTRAINT [FK_AlunoTurma_Turma] FOREIGN KEY([TurmaId])
REFERENCES [dbo].[Turma] ([Id])
GO
ALTER TABLE [dbo].[AlunoTurma] CHECK CONSTRAINT [FK_AlunoTurma_Turma]
GO
ALTER TABLE [dbo].[Disciplina]  WITH CHECK ADD  CONSTRAINT [FK_Disciplina_GrupoDisciplinar] FOREIGN KEY([GrupoDisciplinarId])
REFERENCES [dbo].[GrupoDisciplinar] ([Id])
GO
ALTER TABLE [dbo].[Disciplina] CHECK CONSTRAINT [FK_Disciplina_GrupoDisciplinar]
GO
ALTER TABLE [dbo].[Escola]  WITH CHECK ADD  CONSTRAINT [FK_Escola_Agrupamento] FOREIGN KEY([AgrupamentoId])
REFERENCES [dbo].[Agrupamento] ([Id])
GO
ALTER TABLE [dbo].[Escola] CHECK CONSTRAINT [FK_Escola_Agrupamento]
GO
ALTER TABLE [dbo].[Evento]  WITH CHECK ADD  CONSTRAINT [FK_Evento_Aluno] FOREIGN KEY([AlunoId])
REFERENCES [dbo].[Aluno] ([Id])
GO
ALTER TABLE [dbo].[Evento] CHECK CONSTRAINT [FK_Evento_Aluno]
GO
ALTER TABLE [dbo].[Evento]  WITH CHECK ADD  CONSTRAINT [FK_Evento_Turma] FOREIGN KEY([TurmaId])
REFERENCES [dbo].[Turma] ([Id])
GO
ALTER TABLE [dbo].[Evento] CHECK CONSTRAINT [FK_Evento_Turma]
GO
ALTER TABLE [dbo].[EventoAnexo]  WITH CHECK ADD  CONSTRAINT [FK_EventoAnexo_Evento] FOREIGN KEY([EventoId])
REFERENCES [dbo].[Evento] ([Id])
GO
ALTER TABLE [dbo].[EventoAnexo] CHECK CONSTRAINT [FK_EventoAnexo_Evento]
GO
ALTER TABLE [dbo].[HorarioTurma]  WITH CHECK ADD  CONSTRAINT [FK_HorarioTurma_Sala] FOREIGN KEY([SalaId])
REFERENCES [dbo].[Sala] ([Id])
GO
ALTER TABLE [dbo].[HorarioTurma] CHECK CONSTRAINT [FK_HorarioTurma_Sala]
GO
ALTER TABLE [dbo].[HorarioTurma]  WITH CHECK ADD  CONSTRAINT [FK_HorarioTurma_TurmaDisciplina] FOREIGN KEY([TurmaDisciplinaId])
REFERENCES [dbo].[TurmaDisciplina] ([Id])
GO
ALTER TABLE [dbo].[HorarioTurma] CHECK CONSTRAINT [FK_HorarioTurma_TurmaDisciplina]
GO
ALTER TABLE [dbo].[Memberships]  WITH CHECK ADD  CONSTRAINT [FK_Memberships_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[Memberships] CHECK CONSTRAINT [FK_Memberships_Applications]
GO
ALTER TABLE [dbo].[Memberships]  WITH CHECK ADD  CONSTRAINT [FK_Memberships_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Memberships] CHECK CONSTRAINT [FK_Memberships_Users]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento]  WITH CHECK ADD  CONSTRAINT [FK_OfertaEscolaAgrupamento_Agrupamento] FOREIGN KEY([AgrupamentoId])
REFERENCES [dbo].[Agrupamento] ([Id])
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento] CHECK CONSTRAINT [FK_OfertaEscolaAgrupamento_Agrupamento]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento]  WITH CHECK ADD  CONSTRAINT [FK_OfertaEscolaAgrupamento_AnoLetivo] FOREIGN KEY([AnoLetivoId])
REFERENCES [dbo].[AnoLetivo] ([Id])
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamento] CHECK CONSTRAINT [FK_OfertaEscolaAgrupamento_AnoLetivo]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar]  WITH CHECK ADD  CONSTRAINT [FK_OfertaEscolaGrupo_Grupo] FOREIGN KEY([GrupoDisciplinarId])
REFERENCES [dbo].[GrupoDisciplinar] ([Id])
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar] CHECK CONSTRAINT [FK_OfertaEscolaGrupo_Grupo]
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar]  WITH CHECK ADD  CONSTRAINT [FK_OfertaEscolaGrupo_Oferta] FOREIGN KEY([OfertaEscolaAgrupamentoId])
REFERENCES [dbo].[OfertaEscolaAgrupamento] ([Id])
GO
ALTER TABLE [dbo].[OfertaEscolaAgrupamentoGrupoDisciplinar] CHECK CONSTRAINT [FK_OfertaEscolaGrupo_Oferta]
GO
ALTER TABLE [dbo].[PlanoCurricular]  WITH CHECK ADD  CONSTRAINT [FK_PlanoCurricular_OfertaFormativa] FOREIGN KEY([OfertaFormativaId])
REFERENCES [dbo].[OfertaFormativa] ([Id])
GO
ALTER TABLE [dbo].[PlanoCurricular] CHECK CONSTRAINT [FK_PlanoCurricular_OfertaFormativa]
GO
ALTER TABLE [dbo].[PlanoCurricularDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_PCD_Disciplina] FOREIGN KEY([DisciplinaId])
REFERENCES [dbo].[Disciplina] ([Id])
GO
ALTER TABLE [dbo].[PlanoCurricularDisciplina] CHECK CONSTRAINT [FK_PCD_Disciplina]
GO
ALTER TABLE [dbo].[PlanoCurricularDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_PCD_PlanoCurricular] FOREIGN KEY([PlanoCurricularId])
REFERENCES [dbo].[PlanoCurricular] ([Id])
GO
ALTER TABLE [dbo].[PlanoCurricularDisciplina] CHECK CONSTRAINT [FK_PCD_PlanoCurricular]
GO
ALTER TABLE [dbo].[Professor]  WITH CHECK ADD  CONSTRAINT [FK_Professor_Agrupamento] FOREIGN KEY([AgrupamentoId])
REFERENCES [dbo].[Agrupamento] ([Id])
GO
ALTER TABLE [dbo].[Professor] CHECK CONSTRAINT [FK_Professor_Agrupamento]
GO
ALTER TABLE [dbo].[Professor]  WITH CHECK ADD  CONSTRAINT [FK_Professor_GrupoRecrutamento] FOREIGN KEY([GrupoRecrutamentoId])
REFERENCES [dbo].[GrupoRecrutamento] ([Id])
GO
ALTER TABLE [dbo].[Professor] CHECK CONSTRAINT [FK_Professor_GrupoRecrutamento]
GO
ALTER TABLE [dbo].[ProfessorDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_ProfessorDisciplina_Disciplina] FOREIGN KEY([DisciplinaId])
REFERENCES [dbo].[Disciplina] ([Id])
GO
ALTER TABLE [dbo].[ProfessorDisciplina] CHECK CONSTRAINT [FK_ProfessorDisciplina_Disciplina]
GO
ALTER TABLE [dbo].[ProfessorDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_ProfessorDisciplina_Professor] FOREIGN KEY([ProfessorId])
REFERENCES [dbo].[Professor] ([Id])
GO
ALTER TABLE [dbo].[ProfessorDisciplina] CHECK CONSTRAINT [FK_ProfessorDisciplina_Professor]
GO
ALTER TABLE [dbo].[Profiles]  WITH CHECK ADD  CONSTRAINT [FK_Profiles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Profiles] CHECK CONSTRAINT [FK_Profiles_Users]
GO
ALTER TABLE [dbo].[Roles]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[Roles] CHECK CONSTRAINT [FK_Roles_Applications]
GO
ALTER TABLE [dbo].[Sala]  WITH CHECK ADD  CONSTRAINT [FK_Sala_Escola] FOREIGN KEY([EscolaId])
REFERENCES [dbo].[Escola] ([Id])
GO
ALTER TABLE [dbo].[Sala] CHECK CONSTRAINT [FK_Sala_Escola]
GO
ALTER TABLE [dbo].[Turma]  WITH CHECK ADD  CONSTRAINT [FK_Turma_AnoLetivo] FOREIGN KEY([AnoLetivoId])
REFERENCES [dbo].[AnoLetivo] ([Id])
GO
ALTER TABLE [dbo].[Turma] CHECK CONSTRAINT [FK_Turma_AnoLetivo]
GO
ALTER TABLE [dbo].[Turma]  WITH CHECK ADD  CONSTRAINT [FK_Turma_Escola] FOREIGN KEY([EscolaId])
REFERENCES [dbo].[Escola] ([Id])
GO
ALTER TABLE [dbo].[Turma] CHECK CONSTRAINT [FK_Turma_Escola]
GO
ALTER TABLE [dbo].[Turma]  WITH CHECK ADD  CONSTRAINT [FK_Turma_PlanoCurricular] FOREIGN KEY([PlanoCurricularId])
REFERENCES [dbo].[PlanoCurricular] ([Id])
GO
ALTER TABLE [dbo].[Turma] CHECK CONSTRAINT [FK_Turma_PlanoCurricular]
GO
ALTER TABLE [dbo].[TurmaDiretor]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDiretor_Professor] FOREIGN KEY([ProfessorId])
REFERENCES [dbo].[Professor] ([Id])
GO
ALTER TABLE [dbo].[TurmaDiretor] CHECK CONSTRAINT [FK_TurmaDiretor_Professor]
GO
ALTER TABLE [dbo].[TurmaDiretor]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDiretor_Turma] FOREIGN KEY([TurmaId])
REFERENCES [dbo].[Turma] ([Id])
GO
ALTER TABLE [dbo].[TurmaDiretor] CHECK CONSTRAINT [FK_TurmaDiretor_Turma]
GO
ALTER TABLE [dbo].[TurmaDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDisciplina_Disciplina] FOREIGN KEY([DisciplinaId])
REFERENCES [dbo].[Disciplina] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplina] CHECK CONSTRAINT [FK_TurmaDisciplina_Disciplina]
GO
ALTER TABLE [dbo].[TurmaDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDisciplina_OfertaEscola] FOREIGN KEY([OfertaEscolaAgrupamentoId])
REFERENCES [dbo].[OfertaEscolaAgrupamento] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplina] CHECK CONSTRAINT [FK_TurmaDisciplina_OfertaEscola]
GO
ALTER TABLE [dbo].[TurmaDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDisciplina_PCD] FOREIGN KEY([PlanoCurricularDisciplinaId])
REFERENCES [dbo].[PlanoCurricularDisciplina] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplina] CHECK CONSTRAINT [FK_TurmaDisciplina_PCD]
GO
ALTER TABLE [dbo].[TurmaDisciplina]  WITH CHECK ADD  CONSTRAINT [FK_TurmaDisciplina_Turma] FOREIGN KEY([TurmaId])
REFERENCES [dbo].[Turma] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplina] CHECK CONSTRAINT [FK_TurmaDisciplina_Turma]
GO
ALTER TABLE [dbo].[TurmaDisciplinaProfessor]  WITH CHECK ADD  CONSTRAINT [FK_TDP_Professor] FOREIGN KEY([ProfessorId])
REFERENCES [dbo].[Professor] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplinaProfessor] CHECK CONSTRAINT [FK_TDP_Professor]
GO
ALTER TABLE [dbo].[TurmaDisciplinaProfessor]  WITH CHECK ADD  CONSTRAINT [FK_TDP_TurmaDisciplina] FOREIGN KEY([TurmaDisciplinaId])
REFERENCES [dbo].[TurmaDisciplina] ([Id])
GO
ALTER TABLE [dbo].[TurmaDisciplinaProfessor] CHECK CONSTRAINT [FK_TDP_TurmaDisciplina]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([ApplicationId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Applications]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Roles]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersInRoles_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_UsersInRoles_Users]
GO
ALTER TABLE [dbo].[Evento]  WITH CHECK ADD  CONSTRAINT [CK_Evento_Origem] CHECK  (([AlunoId] IS NOT NULL AND [TurmaId] IS NULL OR [AlunoId] IS NULL AND [TurmaId] IS NOT NULL))
GO
ALTER TABLE [dbo].[Evento] CHECK CONSTRAINT [CK_Evento_Origem]
GO
USE [master]
GO
ALTER DATABASE [GestaoEscolar] SET  READ_WRITE 
GO
