# ♻️ Sistema de Gestão de Resíduos

API RESTful desenvolvida em **ASP.NET Core (.NET 9)** para gerenciamento completo do ciclo de vida de coleta e processamento de resíduos recicláveis e industriais, com persistência em **Microsoft SQL Server**, autenticação via **JWT (JSON Web Token)** e suporte a execução conteinerizada via **Docker**.

---

## 🚀 Tecnologias Utilizadas

- **Plataforma:** .NET 9 (C#)
- **Framework Web:** ASP.NET Core Web API
- **ORM / Banco de Dados:** Entity Framework Core 8.0 & Microsoft SQL Server
- **Documentação Interativa:** Swagger / OpenAPI (Swashbuckle)
- **Segurança:** Autenticação e Autorização via JWT Bearer
- **Testes Automatizados:** xUnit & Moq
- **Conteinerização:** Docker & Docker Compose

---

## 📁 Estrutura do Projeto

```text
├── GestaoResiduos/
│   ├── Controllers/            # Controllers da API (Auth, Usuario, TipoResiduo, Coleta, Processamento)
│   ├── Data/
│   │   ├── Contexts/           # DbContext do Entity Framework (DatabaseContext)
│   │   └── Repositories/       # Camada de Repositórios (Pattern Repository)
│   ├── Migrations/             # Migrações do banco de dados EF Core
│   ├── Models/                 # Entidades de Domínio
│   ├── Services/               # Regras de Negócio e Serviços (ex: AuthService)
│   ├── ViewModel/              # Modelos de transferência de dados (DTOs)
│   ├── Dockerfile              # Imagem Docker da API
│   ├── appsettings.json        # Configurações da aplicação e connection strings
│   └── Program.cs              # Inicialização da aplicação e injeção de dependências
├── GestaoResiduos.UnitTests/   # Projeto de Testes Unitários dos Controllers
├── docker-compose.yml          # Orquestração da API e SQL Server no Docker
└── GestaoResiduos.sln          # Solução do Visual Studio
```

---

## 🗄️ Modelagem de Dados

- **Usuario**: Gerenciamento de operadores, administradores e clientes.
- **TipoResiduo**: Categorização dos resíduos (plásticos, metais, orgânicos), cor de identificação, taxa de reciclagem e métodos de processamento.
- **Coleta**: Registro e agendamento de coletas de resíduos vinculadas ao usuário solicitante.
- **Processamento**: Registro do processamento/reciclagem de uma coleta, calculando a eficiência e a quantidade final produzida.

---

## 🔌 Principais Endpoints da API

### 🔐 Autenticação (`/api/Auth`)
| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/Auth/login` | Realiza login e gera o token JWT |

### 👤 Usuários (`/api/Usuario`)
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Usuario` | Lista todos os usuários *(Requer JWT)* |
| `GET` | `/api/Usuario/{id}` | Busca usuário por ID |
| `GET` | `/api/Usuario/tipo/{tipo}` | Filtra usuários por tipo |
| `POST` | `/api/Usuario` | Cria um novo usuário |
| `PUT` | `/api/Usuario/{id}` | Atualiza dados do usuário |
| `DELETE` | `/api/Usuario/{id}` | Remove um usuário |

### 🏷️ Tipos de Resíduos (`/api/TipoResiduo`)
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/TipoResiduo` | Lista todos os tipos de resíduos *(Requer JWT)* |
| `GET` | `/api/TipoResiduo/{id}` | Busca tipo de resíduo por ID |
| `GET` | `/api/TipoResiduo/reciclaveis` | Lista apenas resíduos recicláveis |
| `POST` | `/api/TipoResiduo` | Cadastra novo tipo de resíduo |
| `PUT` | `/api/TipoResiduo/{id}` | Atualiza um tipo de resíduo |
| `DELETE` | `/api/TipoResiduo/{id}` | Remove um tipo de resíduo |

### 🚛 Coletas (`/api/Coleta`)
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Coleta` | Lista todas as coletas *(Requer JWT)* |
| `GET` | `/api/Coleta/{id}` | Detalhes de uma coleta específica |
| `GET` | `/api/Coleta/estatisticas` | Estatísticas gerais de coletas |
| `POST` | `/api/Coleta` | Agenda/registra uma nova coleta |
| `PUT` | `/api/Coleta/{id}` | Atualiza dados de uma coleta |
| `DELETE` | `/api/Coleta/{id}` | Remove uma coleta (apenas se não processada) |

### ⚙️ Processamento (`/api/Processamento`)
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/Processamento` | Lista os processamentos efetuados *(Requer JWT)* |
| `GET` | `/api/Processamento/{id}` | Detalhes de um processamento específico |
| `GET` | `/api/Processamento/estatisticas` | Estatísticas e médias de eficiência |
| `POST` | `/api/Processamento` | Inicia o processamento de uma coleta |
| `PUT` | `/api/Processamento/{id}` | Atualiza dados de processamento |
| `DELETE` | `/api/Processamento/{id}` | Remove processamento e reverte status da coleta |

---

## ⚙️ Como Subir a Aplicação

A forma mais simples e recomendada é via **Docker Compose**, pois ele sobe a API e o banco de dados SQL Server automaticamente com as migrações aplicadas.

### Opção 1: Execução via Visual Studio (F5)

1. **Pré-requisitos:**
   - [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.
   - Abra a solução `GestaoResiduos.sln` no Visual Studio 2022.

2. **Executar:**
   - No Gerenciador de Soluções (*Solution Explorer*), clique com o botão direito no projeto **`docker-compose`** e selecione **"Definir como Projeto de Inicialização"** (*Set as Startup Project*).
   - Pressione **F5** (ou clique no botão verde **Docker Compose**).
   - O Swagger abrirá automaticamente no navegador em **`http://localhost:8080`**.

---

### Opção 2: Execução via Linha de Comando (Docker Compose)

1. **Pré-requisitos:**
   - [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.

2. **Subir os contêineres:**
   Abra o terminal na pasta raiz do projeto (onde está o `docker-compose.yml`) e execute:
   ```bash
   docker compose up --build -d
   ```

3. **Acessar a API e Documentação:**
   - **Swagger UI:** [http://localhost:8080](http://localhost:8080) ou [http://localhost:8081](http://localhost:8081)

4. **Encerrar os contêineres:**
   ```bash
   docker compose down
   ```

---

### Opção 3: Execução Local (.NET CLI / Visual Studio - HTTP)

Caso queira debugar o código C# da API diretamente na máquina host sem conteinerizar a API:

1. Suba apenas o banco SQL Server no Docker:
   ```bash
   docker compose up -d sqlserver
   ```
2. No Visual Studio, selecione o perfil **`GestaoResiduos` (http)** e pressione **F5** (ou execute via CLI: `dotnet run --project GestaoResiduos/GestaoResiduos`).
3. Acesse o Swagger em: `http://localhost:5294` ou `https://localhost:7010`.

---

## 📖 Como Utilizar a Aplicação (Fluxo no Swagger / Postman)

Para testar os endpoints que exigem autenticação (`[Authorize]`), siga os passos abaixo:

### 1. Obter o Token de Autenticação (Login)
1. No Swagger UI, localize a rota **`POST /api/Auth/login`**.
2. Clique em **Try it out** e utilize um dos usuários pré-cadastrados no corpo da requisição:
   ```json
   {
     "nome": "Antonio Brito",
     "senha": "pass123"
   }
   ```
   *(Ou `"nome": "José Victor"` com senha `"pass123"`)*
3. Clique em **Execute** e copie o valor do campo `"token"` retornado no JSON.

### 2. Autenticar no Swagger UI
1. No topo superior direito da página do Swagger, clique no botão verde **Authorize**.
2. No campo **Value**, digite:
   ```text
   Bearer SEU_TOKEN_AQUI
   ```
3. Clique em **Authorize** e depois em **Close**. Agora todas as requisições protegidas enviarão o header JWT automaticamente.

### 3. Fluxo Recomendado de Operação
1. **Cadastrar Tipo de Resíduo:** `POST /api/TipoResiduo` (ex: Plástico, Metal, Papelão).
2. **Cadastrar Usuário:** `POST /api/Usuario` (ou consulte os existentes via `GET /api/Usuario`).
3. **Agendar uma Coleta:** `POST /api/Coleta` informando o `usuarioId` e o `tipoResiduoId`.
4. **Processar a Coleta:** `POST /api/Processamento` informando o `coletaId`, `usuarioId` e `tipoResiduoId`.
5. **Acompanhar Estatísticas:**
   - `GET /api/Coleta/estatisticas` (totais de coletas e volume recolhido).
   - `GET /api/Processamento/estatisticas` (médias de eficiência e produção gerada).

---

## 🧪 Execução dos Testes Unitários

O Gerenciador de Testes do Visual Studio pode ser acessado através do menu **Testes** -> **Gerenciador de Testes**.

Para rodar todos os testes automatizados da aplicação via terminal em outras IDEs, verifique o caminho da pasta e rode o comando abaixo:

```bash
dotnet test
```
