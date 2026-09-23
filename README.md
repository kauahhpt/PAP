# AlunoGest

Sistema de gestão escolar desenvolvido em **ASP.NET Web Forms (C#)** com **SQL Server**, criado como Projeto de Aptidão Profissional (PAP). Permite a gestão completa de alunos, turmas, professores e encarregados de educação, com dashboards, comunicação e funcionalidades administrativas próprias para cada perfil.

> 🇵🇹 Este README está em português. Para a versão em inglês, ver [secção no final](#english-version).

---

## 📋 Sobre o Projeto

O AlunoGest foi desenvolvido para digitalizar e centralizar a gestão de informação escolar, substituindo processos manuais por um sistema web com diferentes níveis de acesso conforme o perfil do utilizador: **Aluno**, **Professor**, **Encarregado de Educação** e **Agrupamento (administração)**.

## 👥 Perfis e Funcionalidades

### 🎓 Aluno
- Dashboard pessoal com resumo de informação
- Feed de publicações por turma
- Página "Minha Turma"
- Mensagens e chat com professores
- Submissão de trabalhos

### 👨‍🏫 Professor
- Dashboard com gestão de turmas
- Chat com alunos
- Criação de publicações direcionadas
- Gestão de informações de contacto (telefone, etc.)

### 👪 Encarregado de Educação
- Dashboard próprio com acompanhamento do educando
- Calendário sincronizado com o calendário do aluno/professor
- Acesso a informação relevante da turma

### 🏫 Agrupamento (Administração)
- Interface de dashboard agrupado
- Gestão de alunos, turmas e associações aluno-turma
- Criação de contas com geração automática de username e password
- Gestão de encarregados de educação e professores

## 🔐 Funcionalidades Transversais

- Autenticação e gestão de contas via **ASP.NET Membership**
- Recuperação de password por token, com envio de email (SMTP)
- Validação de NIF
- Calendário integrado entre perfis
- Sistema de chat entre utilizadores
- Feed de publicações por turma/agrupamento

## 🛠️ Tecnologias

- **Backend:** C#, ASP.NET Web Forms
- **Base de Dados:** SQL Server (SQL Server Express)
- **Frontend:** Web Forms (.aspx), CSS
- **Autenticação:** ASP.NET Membership
- **Email:** SMTP (Gmail)
- **Controlo de Versão:** Git / GitHub

## 📸 Capturas de Ecrã

<img width="1869" height="995" alt="Dashboard" src="https://github.com/user-attachments/assets/b821ce69-66c7-4a46-9a17-b1f83b1be291" />

<img width="1860" height="990" alt="Login" src="https://github.com/user-attachments/assets/88e6e9e4-b195-4346-8ccc-29d0b70a5272" />

<img width="1866" height="987" alt="Dashboard do Aluno" src="https://github.com/user-attachments/assets/695cf7cf-b1a9-454e-be6b-92aa93fcef08" />
<img width="1864" height="990" alt="Dashboard do Professor" src="https://github.com/user-attachments/assets/45c25c72-30d7-48a3-a22a-dd75853bab66" />

<img width="1867" height="992" alt="Dashboard do Encarregado de Educação" src="https://github.com/user-attachments/assets/53d36190-a3eb-403d-82cf-e3317c6c4c9b" />

<img width="1862" height="996" alt="Chat" src="https://github.com/user-attachments/assets/6f0b3781-1bb4-4e3f-9ade-7fb85eac8a84" />

## 📁 Estrutura do Projeto

```
AlunoGest/
├── agrupamento/       # Páginas e lógica do perfil Agrupamento (admin)
├── aluno/              # Páginas e lógica do perfil Aluno
├── professor/          # Páginas e lógica do perfil Professor
├── encarregado/        # Páginas e lógica do perfil Encarregado de Educação
├── Util/                # Classes utilitárias (validações, criação de contas, etc.)
├── login.aspx           # Página de autenticação
└── recuperar_password.aspx  # Recuperação de password
```

## 🚀 Como Executar

### Pré-requisitos
- Visual Studio (2019 ou superior)
- SQL Server Express (ou SQL Server completo)
- .NET Framework compatível com Web Forms

### Passos

1. Clona o repositório:
   ```bash
   git clone https://github.com/kauahhpt/PAP.git
   ```
2. Abre a solução (`.sln`) no Visual Studio
3. Configura a connection string no `Web.config` para apontar para a tua instância local de SQL Server
4. Corre o script de criação da base de dados (disponível na pasta do projeto)
5. Configura as credenciais de SMTP (Gmail) no `Web.config`, caso queiras testar o envio de emails
6. Executa o projeto (F5)

## 👤 Autor

**Kauã Hipólito**
- GitHub: [@kauahhpt](https://github.com/kauahhpt)
- Portfólio: [kauahhpt.github.io/Curriculo](https://kauahhpt.github.io/Curriculo/)

---

## English Version

# AlunoGest

A school management system built with **ASP.NET Web Forms (C#)** and **SQL Server**, developed as a final professional project (PAP). It provides student, class, teacher, and guardian management through role-specific dashboards and communication tools.

### User Roles
- **Student:** personal dashboard, class feed, messaging/chat with teachers, assignment submission
- **Teacher:** class management dashboard, chat with students, targeted posts
- **Guardian:** dedicated dashboard, calendar synced with student/teacher calendars
- **Admin (Agrupamento):** student/class management, account creation with automatic username/password generation, teacher and guardian management

### Key Features
- Authentication via ASP.NET Membership
- Token-based password recovery with SMTP email delivery
- NIF (tax ID) validation
- Cross-role synced calendar
- Class-based post feed and chat system

### Tech Stack
C#, ASP.NET Web Forms, SQL Server, ASP.NET Membership, SMTP (Gmail)

### Getting Started
1. Clone: `git clone https://github.com/kauahhpt/PAP.git`
2. Open the `.sln` in Visual Studio
3. Update the connection string in `Web.config` to point to your local SQL Server instance
4. Run the included database creation script
5. Configure SMTP credentials in `Web.config` if you want to test email delivery
6. Run the project (F5)

### Author
**Kauã Hipólito** — [GitHub](https://github.com/kauahhpt) · [Portfolio](https://kauahhpt.github.io/Curriculo/)
