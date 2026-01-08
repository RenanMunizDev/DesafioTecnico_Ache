# Desafio Técnico Aché - Integração SAP S/4HANA SD

## ?? Descrição

API REST desenvolvida em **.NET 8 (LTS)** para integração com o módulo **SAP S/4HANA SD (Sales & Distribution)** utilizando **OData REST API**. A solução implementa operações de **GET** e **POST** para gerenciamento de pedidos de venda com dados mockados simulando integração real com SAP.

## ??? Arquitetura

**Padrão**: Clean Architecture + CQRS

```
src/
??? DesafioTecnico_Ache.API/          # Camada de Apresentação
?   ??? Controllers/                   # Controllers REST
?   ??? Middleware/                    # Security & Error Handling
?   ??? Program.cs                     # DI Container & Startup
??? DesafioTecnico_Ache.Application/  # Camada de Aplicação
?   ??? Commands/                      # Write Operations (CQRS)
?   ??? Queries/                       # Read Operations (CQRS)
?   ??? DTOs/                          # Data Transfer Objects
??? DesafioTecnico_Ache.Domain/       # Camada de Domínio
?   ??? Entities/                      # Domain Models
?   ??? Interfaces/                    # Abstractions
??? DesafioTecnico_Ache.Infrastructure/ # Camada de Infraestrutura
    ??? Repositories/                  # Data Access
    ??? SAP/                           # SAP Integration (Mock)
```

### Separação de Responsabilidades

- **API**: Controllers, Middlewares, Validações de entrada
- **Application**: Use cases (Commands/Queries), Orquestração, DTOs
- **Domain**: Regras de negócio, Entidades, Interfaces
- **Infrastructure**: Acesso a dados, Integrações externas (SAP)

## ?? Segurança (OWASP Top 10 - 2021)

#### Implementações de Segurança:

1. **Authentication & Authorization**
   - API Key authentication via header `X-API-Key`
   - Middleware customizado `ApiKeyAuthenticationMiddleware`

2. **Security Headers** (OWASP A05:2021 - Security Misconfiguration)
   - `X-Content-Type-Options: nosniff`
   - `X-Frame-Options: DENY`
   - `X-XSS-Protection: 1; mode=block`
   - `Referrer-Policy: no-referrer`
   - `Permissions-Policy` restritivo
   - Remoção de headers que expõem informações (`Server`, `X-Powered-By`)

3. **Input Validation** (OWASP A03:2021 - Injection)
   - Data Annotations para validação de entrada
   - Validação de modelo via ModelState
   - Sanitização de parâmetros

4. **CORS Configuration** (OWASP A05:2021)
   - Whitelist de origens permitidas
   - Headers e métodos HTTP restritos
   - Configuração segura de credenciais

5. **Rate Limiting** (OWASP A04:2021 - Insecure Design)
   - 100 requisições por minuto por API Key
   - Proteção contra DDoS e brute force

6. **HTTPS Enforcement**
   - Redirecionamento automático para HTTPS
   - TLS 1.2+ obrigatório em produção

7. **Sensitive Data Exposure** (OWASP A02:2021)
   - Senhas não armazenadas em código
   - Configurações sensíveis via appsettings/environment variables
   - Logging seguro sem exposição de dados sensíveis

### ?? Integração SAP S/4HANA

**Tipo de Integração**: OData REST API (Mockado para demonstração)

**Endpoint SAP**: `/sap/opu/odata/sap/API_SALES_ORDER_SRV`

**Operações Simuladas**:
- `GET /A_SalesOrder('{orderId}')` - Buscar pedido por ID
- `POST /A_SalesOrder` - Criar novo pedido de venda

**Autenticação SAP**: Basic Authentication (credenciais configuráveis)

**Nota sobre Mock**: A implementação utiliza armazenamento em memória compartilhado entre requests para simular persistência. Em produção, seria substituída por chamadas reais ao SAP OData API.

### ?? Endpoints da API

#### 1. GET - Buscar Pedido de Venda
```http
GET /api/v1/salesorders/{orderId}
Headers:
  X-API-Key: SAP-API-KEY-DEMO-2026-ACHE-DESAFIO
```

**Resposta 200 OK**:
```json
{
  "orderId": "SO20260108001",
  "customerCode": "CUST001",
  "customerName": "Farmácia Popular Ltda",
  "orderDate": "2026-01-03T00:00:00Z",
  "totalAmount": 437.50,
  "status": "CONFIRMED",
  "items": [
    {
      "itemId": "ITM001",
      "materialCode": "MAT001",
      "materialDescription": "Paracetamol 500mg",
      "quantity": 100,
      "unitPrice": 2.50,
      "totalPrice": 250.00
    }
  ]
}
```

#### 2. POST - Criar Pedido de Venda
```http
POST /api/v1/salesorders
Headers:
  X-API-Key: SAP-API-KEY-DEMO-2026-ACHE-DESAFIO
  Content-Type: application/json

Body:
{
  "customerCode": "CUST004",
  "customerName": "Farmácia Nova",
  "status": "PENDING",
  "items": [
    {
      "materialCode": "MAT001",
      "materialDescription": "Paracetamol 500mg",
      "quantity": 50,
      "unitPrice": 2.50
    }
  ]
}
```

**Resposta 201 Created**:
```json
{
  "orderId": "SO20260108004",
  "customerCode": "CUST004",
  "customerName": "Farmácia Nova",
  "orderDate": "2026-01-08T12:00:00Z",
  "totalAmount": 125.00,
  "status": "PENDING",
  "items": [
    {
      "itemId": "ITMF2A3B4C5",
      "materialCode": "MAT001",
      "materialDescription": "Paracetamol 500mg",
      "quantity": 50,
      "unitPrice": 2.50,
      "totalPrice": 125.00
    }
  ]
}
```

**Formato do Order ID**: `SO{yyyyMMdd}{nnn}` onde:
- `SO`: Prefixo padrão SAP Sales Order
- `yyyyMMdd`: Data atual (ex: 20260108)
- `nnn`: Contador sequencial de 3 dígitos (ex: 001, 002, 003, 004...)

### ?? Como Executar

1. **Restaurar dependências**:
```bash
dotnet restore
```

2. **Build da solução**:
```bash
dotnet build
```

3. **Executar a API**:
```bash
cd src/DesafioTecnico_Ache.API
dotnet run
```

4. **Acessar Swagger**:
   - Development: `https://localhost:7000/swagger` ou a porta indicada no console

### ?? Testes com Swagger

1. Acesse o Swagger UI
2. Clique em "Authorize"
3. Insira a API Key: `SAP-API-KEY-DEMO-2026-ACHE-DESAFIO`
4. Teste os endpoints GET e POST

**Fluxo de Teste Recomendado**:
1. GET `/api/v1/salesorders/SO20260108001` - Buscar pedido mock existente
2. POST `/api/v1/salesorders` - Criar novo pedido (retornará ID como `SO20260108004`)
3. GET `/api/v1/salesorders/SO20260108004` - Buscar o pedido recém-criado

### ?? Dados Mock Disponíveis

A API já vem com 3 pedidos de venda pré-cadastrados para teste:
- `SO20260108001` - Farmácia Popular Ltda (CONFIRMED) - 2 itens
- `SO20260108002` - Drogaria Moderna S.A. (PROCESSING) - 1 item
- `SO20260108003` - Rede Saúde Plus (PENDING) - 3 itens

**Importante**: Os pedidos criados via POST são persistidos em memória durante toda a execução da aplicação (compartilhados entre requests). Ao reiniciar a API, apenas os 3 pedidos mock estarão disponíveis novamente.

### ?? Princípios Aplicados

- **SOLID**: Princípios de responsabilidade única, inversão de dependência, etc.
- **DRY (Don't Repeat Yourself)**: Reutilização através de abstrações
- **KISS (Keep It Simple, Stupid)**: Código simples e direto
- **YAGNI (You Aren't Gonna Need It)**: Implementação apenas do necessário
- **Clean Code**: Nomenclatura clara, métodos pequenos, baixo acoplamento

### ?? Notas de Implementação

1. **SAP OData Service**: Mock implementado para simular chamadas reais ao SAP
2. **Repository Pattern**: Facilita testes e manutenção
3. **CQRS**: Separação clara entre leitura (Queries) e escrita (Commands)
4. **Validation**: Múltiplas camadas de validação (DTO Annotations, Domain, Application)
5. **Logging**: Estruturado e contextualizado para troubleshooting
6. **Error Handling**: Tratamento consistente com status HTTP apropriados
7. **Thread-Safety**: Operações protegidas com locks para ambiente multi-thread
8. **ID Generation**: Sequencial e consistente com formato SAP padrão

### ?? Melhorias Futuras para Produção

- Implementar autenticação OAuth 2.0 / JWT
- Adicionar cache distribuído (Redis)
- Implementar circuit breaker para chamadas SAP (Polly)
- Adicionar testes unitários e de integração (xUnit)
- Implementar API Gateway
- Adicionar observabilidade (OpenTelemetry, Application Insights)
- Implementar versionamento de API
- Adicionar documentação OpenAPI completa
- Substituir armazenamento in-memory por banco de dados (SQL Server / SAP HANA)
- Implementar padrão Saga para transações distribuídas
- Adicionar health checks para monitoramento

---

**Desenvolvido seguindo as melhores práticas de arquitetura de software e segurança OWASP**
