# Gestão de Consultas - UnitSaude - App Mobile

 Esta aplicação foi desenvolvida como parte da disciplina de Desenvolvimento Mobile, do 5º Período do
curso de Análise e Desenvolvimento de Sistemas, no Centro Universitário Tiradentes - Unit PE.
 Com o objetivo de facilitar e automatizar processos manuais de agendamento da clínica universitária
integrada.

Professor: Diógenes Carvalho Matias.

## Stack Tecnológica

- Web API: ASP .Net Core (C#);
- Comunicação com Web API: Retrofit;
- App Mobile: Android Studio (Java).

## Funcionalidades

- Autenticação: Permite que pacientes cadastrados façam login e acessem os serviços disponíveis.
  
- Recuperação de senha: Permite que pacientes cadastrados recuperem suas senhas.
  
- Controle de Acesso: Garante que pacientes possam acessar apenas os serviços aos quais têm
  permissão.
  
- Agendamento de Consultas: Sistema para visualizar e agendar consultas;

## Como Utilizar

- Baixar o aplicativo: (Atualmente indisponível).

- Ou utilize localmente:

  - Clone o repositório com o comando:
    ```
    git clone https://github.com/Louiexz/agendamentos-de-consultas.git
    ```
  
  - Caso use softare para desenvolvimento, abra a pasta nele e execute o projeto.
 
  - Ou execute pelo terminal:
 
    - cd agendamento-de-consultas/Mobile;
    - ./gradlew installDebug (Se estiver com um celular android conectado via depuração usb).
    - Ou ./gradlew assembleDebug (Gera o apk para debug) depois cd app/build/outputs/apk/debug, baixe e instale o app-debug.apk no seu celular.

## Estrutura do projeto

    Mobile/app/
      ├── docs/
      │   └── UnitSaude_Documentacao.pdf
      |
      ├── diagrams/
      |   ├── er_diagram.pdf
      │   ├── logical_diagram.pdf
      │   └── physical_diagram.pdf
      |
      └── src/main/java/com/example/ # Caminho para o root
          ├── unitsaude/         # Diretório principal
          │   ├── data/              # Gerencia dados e comunicação com fontes externas e locais (APIs, bd's, cache)
          │   │   ├── api/               # Requisições externas (APIs)
          │   │   ├── dto/               # Modelos de dados (Data Transfer Objects)
          │   │   ├── local/             # Dados locais (cache, banco de dados)
          │   │   └── repositorio/       # Interação com dados, combinando APIs e fontes locais
          │   │
          │   ├── activities/        # Componentes visuais reutilizáveis e adaptação de dados para exibição
          │   ├── utils/             # Funções auxiliares (validações, checagem de internet, manipulação de API)
          │   ├── viewmodel/         # Lógica de negócios, integrando repositórios e telas
          │   │
          │   └── MainActivity.java  # Classe principal, configurações iniciais e integração do fluxo de dados
          │
          ├── res/               # Contém recursos, como layouts, imagens, strings, cores e estilos.
          │
          └── AndroidManifest.xml   # Declara permissões, atividades, serviços e outros componentes.

## Contribuições

- Para contribuir com o projeto, siga os passos abaixo:

    - Clone o repositório: Faça um fork do repositório e clone-o para o seu ambiente local.

    - Crie uma nova branch: Crie uma branch com um nome descritivo para a sua contribuição (exemplo:
      feature/nome-da-funcionalidade ou bugfix/correção-erro).

    - Realize suas alterações: Implemente suas melhorias ou correções de forma clara e organizada.

    - Faça testes: Verifique se as suas mudanças não quebram funcionalidades existentes e se a
      aplicação se comporta conforme esperado.

    - Crie um Pull Request: Após suas alterações, abra um pull request com uma descrição clara das
      mudanças realizadas.

- Todas as contribuições serão analisadas cuidadosamente pela nossa equipe. Buscamos manter a
  qualidade do código e garantir que as contribuições estejam alinhadas com os objetivos do projeto.

## Autores

Artur Ramos    - [@4rturr](https://github.com/4rturr) -<br>
Carlos Eduardo - [@carlos-1ima](https://github.com/carlos-1ima) -<br>
Luiz Augusto   - [@Louiexz](https://github.com/Louiexz) - 1231309870<br>
Paulo Arthur   - [@pauludelimaa](https://github.com/pauludelimaa) -<br>
Vinicius José  - [@ViniciusRKX](https://github.com/ViniciusRKX) -
