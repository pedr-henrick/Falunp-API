# Falunp API

## Descrição

A Falunp API é uma aplicação RESTful que simula o gerenciamento de uma secretaria educacional, permitindo o cadastro, listagem, edição e exclusão de alunos, turmas e matrículas. A API inclui autenticação segura para administradores e é construída com .NET Core 8, Entity Framework Core e SQL Server, seguindo boas práticas como Clean Architecture e observabilidade com OpenTelemetry.

## Tecnologias Utilizadas

- **Framework**: .NET 8.0
- **Banco de Dados**: SQL Server
- **ORM**: Entity Framework Core
- **Autenticação**: JWT Bearer
- **Documentação**: Swagger/OpenAPI
- **Validações**: FluentValidation
- **Mapeamento**: Mapster
- **Observabilidade**: OpenTelemetry
- **Geração de Dados**: Bogus
- **Testes**: Moq, Shouldly (89 testes unitários)
- **Hash de Senha**: BCrypt
- **Logging**: Serilog

## Funcionalidades Implementadas

- **Gerenciamento de Alunos**: Suporte para adicionar, listar, editar e remover alunos, com validações básicas e autenticação.
- **Gerenciamento de Turmas**: Capacidade de criar, listar, atualizar e deletar turmas, com funcionalidades associadas.
- **Gerenciamento de Matrículas**: Permite associar alunos a turmas e possivelmente visualizar essas associações.
- **Autenticação Segura**: Login de administradores com JWT, utilizando senhas criptografadas com BCrypt.
- **Documentação Interativa**: Interface Swagger para explorar e testar as APIs.

## Arquitetura e Design Patterns

A API é estruturada com **Clean Architecture**, separando camadas de domínio, serviços, repositórios e controladores para garantir coesão e baixo acoplamento. O **Repository Pattern** é implementado para abstrair o acesso a dados, facilitando a manutenção e escalabilidade com Entity Framework Core. Embora padrões como CQRS (Command Query Responsibility Segregation) tenham sido mencionados em contextos externos, esta implementação optou por uma abordagem unificada sem separação explícita de comandos e consultas. O **Result Pattern** é utilizado para respostas consistentes, e o **Problem Details** assegura um tratamento uniforme de erros. Princípios de **Clean Code** são aplicados para promover legibilidade e manutenibilidade.

## Pré-requisitos

- .NET 8.0 SDK
- SQL Server (local ou Azure)
- Visual Studio 2022 ou VS Code com C# extension
- Postman ou similar para testes de API

## Instalação e Execução

### 1. Clonar o Repositório
```bash
git clone https://github.com/pedr-henrick/Falunp-API.git
cd Falunp-API
git checkout feat/addloginRoute
```

### 2. Configurar o Banco de Dados
- Execute o script `dump.sql` no SQL Server para criar o banco (se disponível) e inserir dados iniciais.
- Atualize a connection string em `appsettings.json` (exemplo):
  ```
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SQL_FALUNP;User Id=sa;Password=Senha@123;TrustServerCertificate=True;"
  }
  ```

### 3. Configurar JWT
- Em `appsettings.json`, defina as chaves JWT:
  ```
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-super-segura-com-pelo-menos-32-chars",
    "Issuer": "Falunp_API",
    "Audience": "Falunp_Admin",
    "ExpiryInMinutes": 60
  }
  ```

### 4. Restaurar Pacotes e Executar
```bash
dotnet restore
dotnet build
dotnet run
```

A API estará disponível em `https://localhost:7265` (ou porta configurada). Acesse Swagger em `https://localhost:7265/swagger`.

### Testes
Para rodar os 89 testes unitários:
```bash
dotnet test
```

## Docker (Opcional)
```bash
docker-compose up -d
```
O `docker-compose.yml` irá com uma imagem do sql server caso seja necessário o uso.

## Contribuições
Abra issues ou PRs para melhorias.
