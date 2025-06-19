# UnitSaude

## Descrição

Sistema para gerenciamento de consultas, com foco na clínica médica do Centro Universitário Tiradentes (UNIT - PE).
Análise e Desenvolvimento de Sistemas, 5º Período.
Entrega final das disciplinas: experiência extensionista, desenvolvimento web, seguro e mobile.

## Tecnologias Utilizadas

- Render para deploy
- .NET 8 como framework
- C# como linguagem
- HangFire para gerenciamento de filas
- PostgreSQL para banco de dados
- Swagger para documentação da API
- Autenticação via Bearer Token (JWT)

## Instalação e Configuração
1. Clone o repositório:
   ```sh
   git clone https://github.com/Louiexz/BackendNoDoc.git -b BackendNoDoc --single-branch
   ```
2. Navegue até o diretório do projeto:
   ```sh
   cd UnitSaude
   ```
3. Configure as variáveis de ambiente no arquivo `appsettings.json`:
   ```json
   {
    "ConnectionStrings":{
      "DefaultConnection":"mongodb://localhost:27017",
    },
    "EmailSettings": {
      "Porta":"587",
      "Servidor":"smtp.gmail.com",
      "UsarSSL":"true",
      "Usuario":"seu@email",
      "Senha":"sua senha",
      "UrlRedefinicaoSenha":"http://localhost:8080/redefinirSenha",
    },
    "Jwt":{
      "Audience":"Users",
      "Issuer":"Users",
      "Key":"sua_key_aqui",
    },
    "SecretKey":"secret_key_forte",
    }
   ```
4. Execute a aplicação:
   ```sh
   dotnet run
   ```

## Endpoints Principais e Públicas

### Usuários
- `POST /api/Usuario/login`: Autenticar o usuário.
- `POST /api/Usuario/redefinir-senha`: Redefinir senha do usuário.
- `POST /api/Usuario/recuperar-senha`: Recuperar senha pelo e-mail recebido.
  
## Testando a API

- Utilize o Swagger para testar os endpoints diretamente:
  ```sh
  http://localhost:8080/swagger/index.html
  ```
  - Para autenticação, utilize o token JWT obtido no endpoint de login.

- Ou utilize o Postman para testar os endpoints manualmente.

## Contribuição

Artur Ramos - [@4rturr](https://github.com/4rturr)<br>
Carlos Eduardo - [@carlos-1ima](https://github.com/carlos-1ima)<br>
Luiz Augusto - [@Louiexz](https://github.com/Louiexz)<br>
Paulo Arthur - [@pauludelimaa](https://github.com/pauludelimaa)<br>
Vinicius José - [@ViniciusRKX](https://github.com/ViniciusRKX)

Contribuições restritas! Analisaremos issues e pull requests.
