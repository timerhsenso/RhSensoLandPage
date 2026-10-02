# RhSenso.Web

Base da nova landing page RhSenso em ASP.NET Core MVC (.NET 9) + PostgreSQL 17.

## O que já está estruturado
- Landing page original migrada para Razor/MVC, preservando CSS e JavaScript.
- PostgreSQL via Entity Framework Core/Npgsql.
- ASP.NET Core Identity.
- Perfil/role `CEO` e rota protegida `/admin`.
- Login em `/acesso`.
- Entidades-base `ContactRequest` e `JobApplication` para as próximas etapas.
- Seção "Trabalhe conosco" preparada na landing.
- Docker Compose para PostgreSQL local.

## Antes de executar
1. Coloque o logo em `wwwroot/img/logo-rhsenso.png` (o ZIP original referencia esse arquivo, mas ele não veio no pacote).
2. Ajuste `ConnectionStrings:DefaultConnection` em `appsettings.json` ou, preferencialmente, via variável de ambiente/Secret Manager.
3. Para criar o primeiro CEO, configure `BootstrapAdmin:Email` e `BootstrapAdmin:Password`. Em produção, use variáveis de ambiente e remova a senha depois do primeiro provisionamento.
4. Crie a migration inicial:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

> O projeto chama `Database.MigrateAsync()` na inicialização. É necessário gerar a migration `InitialCreate` antes da primeira execução.

## PostgreSQL local opcional
```bash
docker compose up -d
```
Nesse caso use `Password=postgres` na connection string local.

## Próximas implementações sugeridas
1. Formulário de contato persistido no PostgreSQL + envio SMTP.
2. Upload seguro de currículo (PDF/DOCX), validação de tamanho/extensão e armazenamento.
3. Listagem/status de contatos e currículos no painel CEO.
4. Configuração SMTP e templates de e-mail.
5. Auditoria, antiforgery, rate limiting/CAPTCHA e política LGPD para currículos.
