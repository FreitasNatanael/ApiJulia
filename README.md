# 🛍️ Desafio Técnico: API de Gerenciamento de Vendas (ApiJulia)

Esta é uma Web API robusta para o gerenciamento de um ecossistema de vendas. O projeto abrange desde o cadastro de clientes e catálogo de produtos até a inteligência complexa de processamento de pedidos com controle de estoque em tempo real.

---

### 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET 8.0)
* **Banco de Dados:** SQL Server (Dockerizado)
* **ORM:** Entity Framework Core (Code First)
* **Documentação:** Swagger UI (OpenAPI)
* **Testes:** Insomnia / Postman

---

### 🧠 Regras de Negócio e Inteligência da API

O diferencial desta aplicação está no controlador de **Pedidos (Orders)**, onde implementei:

* ✅ **Validação de Carrinho:** Impede a criação de pedidos sem itens.
* ✅ **Gestão de Estoque:** Valida a disponibilidade antes da venda. Se `Quantidade Solicitada > Estoque`, a API retorna um erro claro.
* ✅ **Segurança de Preço:** O valor unitário é capturado do banco no momento do pedido, evitando que o cliente altere o preço via JSON (proteção contra fraudes).
* ✅ **Cálculo de Totalizador:** Soma automática de todos os itens (`Quantidade x Preço Unitário`).
* ✅ **Baixa em Tempo Real:** O estoque do produto é decrementado automaticamente após a confirmação do pedido.
* ✅ **Precisão Decimal:** Configuração de `decimal(18,2)` para garantir que nenhum centavo seja perdido em arredondamentos.

---

### 🚀 Como Executar

#### 1. Pré-requisitos
* Visual Studio 2022.
* Docker Desktop (para o SQL Server).

#### 2. Configurar o Banco
Com o Docker Desktop aberto, execute no **Console do Gerenciador de Pacotes** (Ferramentas > Gerenciador de Pacotes NuGet > Console do Gerenciador de Pacotes):

```bash
docker compose up -d
```

Após o container estar rodando, execute:

```bash
Update-Database
```

#### 3. Rodar
Aperte **F5**. O Swagger abrirá em: `http://localhost:5200/swagger/index.html`. Caso não abra automaticamente, copie e cole a URL no seu navegador.

### 🔌 Endpoints da API

#### **👥 Clientes (Customers)**
| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **GET** | `/api/Customers` | Lista todos os clientes cadastrados |
| **GET** | `/api/Customers/{id}` | Busca os detalhes de um cliente específico |
| **POST** | `/api/Customers` | Cadastra um novo cliente |
| **PUT** | `/api/Customers/{id}` | Atualiza os dados de um cliente existente |
| **DELETE** | `/api/Customers/{id}` | Remove um cliente do sistema |

#### **📦 Produtos (Products)**
| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **GET** | `/api/Products` | Lista catálogo e saldo atual de estoque |
| **POST** | `/api/Products` | Adiciona novos produtos ao inventário |

#### **🛒 Pedidos (Orders)**
| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **GET** | `/api/Orders` | Lista todos os pedidos com seus respectivos itens |
| **GET** | `/api/Orders/{id}` | Busca um pedido específico detalhado |
| **POST** | `/api/Orders` | **Cria pedido, calcula total e abate estoque** |

---

### 📄 Exemplo de Payload (Pedido)

**POST** `/api/Orders`
```json
{
  "customerId": 1,
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

---

### 📄 Exemplos de Resposta

#### Sucesso (201 Created)
```json
{
  "id": 10,
  "date": "2023-10-27T14:30:00",
  "customerId": 1,
  "total": 7000.00,
  "items": [
    {
      "id": 15,
      "productId": 1,
      "quantity": 2,
      "unitPrice": 3500.00
    }
  ]
}
```

#### Erro: Sem Estoque (400 Bad Request)
> Quando o cliente tenta comprar mais do que há disponível na prateleira.

```json
{
  "error": "Produto sem estoque suficiente"
}
```

---
**Desenvolvido por Natanael Freitas**
