# Documentação da API Sabires

> Resumo das rotas da API Sabires, organizado por funcionalidade.

-   **Repositório:** https://github.com/Sabires-Edu/BackEnd-Application
-   **Swagger:** https://sabires.onrender.com/swagger/index.html
-   **Swagger JSON:**
    https://sabires.onrender.com/swagger/v1/swagger.json

## 1. Informações gerais

  Propriedade        Valor
  ------------------ --------------------------------
  URL base           `https://sabires.onrender.com`
  Versão da API      `v1`
  Formato de dados   JSON
  Autenticação       JWT Bearer
  Paginação padrão   Página 1, limite 10

Para rotas protegidas, envie o token JWT no cabeçalho `Authorization`:

``` http
Authorization: Bearer SEU_TOKEN_JWT
```

O token é retornado no campo `response.jwt` após o cadastro ou login. As
rotas de cadastro e login não exigem autenticação. As demais rotas de
usuários, publicações e chats exigem um token válido, conforme a
implementação dos controllers. A rota de verificação de saúde também é
pública.

## 2. Usuários e autenticação

Prefixo: `/v1/users`

  Método   Endpoint                  Descrição
  -------- ------------------------- -----------------------------------------------
  POST     `/v1/users`               Cadastra um usuário e retorna o JWT.
  POST     `/v1/users/me/login`      Realiza o login e retorna o JWT.
  GET      `/v1/users/me`            Retorna os dados do usuário autenticado.
  GET      `/v1/users`               Lista os usuários cadastrados, com paginação.
  GET      `/v1/users/{userid}`      Consulta um usuário pelo identificador.
  PATCH    `/v1/users/me`            Atualiza os dados do usuário autenticado.
  PATCH    `/v1/users/me/password`   Altera a senha do usuário.
  PUT      `/v1/users/me/picture`    Atualiza a foto de perfil.

### Seguidores

  ------------------------------------------------------------------------------------------
  Método                  Endpoint                                   Descrição
  ----------------------- ------------------------------------------ -----------------------
  POST                    `/v1/users/me/follows/{followinguserid}`   Solicita seguir outro
                                                                     usuário.

  GET                     `/v1/users/{userid}/followers`             Lista os seguidores de
                                                                     um usuário.

  GET                     `/v1/users/{userid}/followings`            Lista os usuários que
                                                                     alguém segue.

  GET                     `/v1/users/me/follows/pending`             Lista as solicitações
                                                                     de seguidores
                                                                     pendentes.

  PATCH                   `/v1/users/me/follows/{followeruserid}`    Aceita ou recusa uma
                                                                     solicitação de
                                                                     seguidor.
  ------------------------------------------------------------------------------------------

Os endpoints de seguidores utilizam identificadores de usuários no
formato UUID. As listagens de seguidores, usuários seguidos e
solicitações pendentes aceitam paginação.

### Dados de cadastro

O endpoint `POST /v1/users` recebe um objeto `CreateUserDTO`:

``` json
{
  "firstName": "André",
  "lastName": "Garcia",
  "email": "andre@example.com",
  "password": "senha",
  "birthDate": "2000-01-01T00:00:00Z",
  "statusId": 1
}
```

Os campos são obrigatórios. O `statusId` representa o nível de
escolaridade do usuário.

O login recebe `email` e `password`. A atualização de perfil aceita
`firstName`, `lastName`, `birthDate`, `statusId` e `biography`, todos
opcionais.

A atualização de senha recebe `currentPassword`, `newPassword` e
`confirmNewPassword`.

A atualização da foto utiliza `multipart/form-data`, com o campo `file`.

## 3. Publicações e comentários

Prefixo: `/v1/posts`

  -----------------------------------------------------------------------------------
  Método                  Endpoint                            Descrição
  ----------------------- ----------------------------------- -----------------------
  POST                    `/v1/posts`                         Cria uma publicação.

  GET                     `/v1/posts`                         Lista as publicações,
                                                              com paginação.

  GET                     `/v1/posts/{id}`                    Consulta uma publicação
                                                              pelo ID.

  GET                     `/v1/posts/{id}/comments`           Lista os comentários de
                                                              uma publicação.

  GET                     `/v1/posts/me`                      Lista as publicações do
                                                              usuário autenticado.

  GET                     `/v1/posts/me/likes`                Lista as publicações
                                                              curtidas pelo usuário.

  GET                     `/v1/posts/me/trash`                Lista as publicações na
                                                              lixeira.

  POST                    `/v1/posts/me/like/{id}`            Curte uma publicação.

  DELETE                  `/v1/posts/me/dislike/{id}`         Remove a curtida de uma
                                                              publicação.

  POST                    `/v1/posts/me/trash/{id}`           Move uma publicação
                                                              para a lixeira.

  DELETE                  `/v1/posts/me/trash/restore/{id}`   Restaura uma publicação
                                                              da lixeira.

  DELETE                  `/v1/posts/me/trash/remove/{id}`    Remove definitivamente
                                                              uma publicação da
                                                              lixeira.
  -----------------------------------------------------------------------------------

### Criação de publicação

O endpoint `POST /v1/posts` recebe `multipart/form-data` com os
seguintes campos:

  -----------------------------------------------------------------------
  Campo                   Tipo                    Descrição
  ----------------------- ----------------------- -----------------------
  `Content`               string                  Conteúdo da publicação.

  `RootPostId`            int                     Identificador da
                                                  publicação original,
                                                  quando aplicável.

  `ParentPostId`          int                     Identificador da
                                                  publicação à qual o
                                                  comentário responde.

  `Images`                arquivo\[\]             Imagens anexadas à
                                                  publicação.
  -----------------------------------------------------------------------

Os campos `RootPostId` e `ParentPostId` são opcionais.

A estrutura permite criar publicações independentes, comentários e
respostas a comentários. Quando uma publicação é criada, a API retorna
seu identificador.

As rotas de listagem de publicações, comentários, curtidas e lixeira
aceitam `page` e `limit`, com valores padrão de 1 e 10, respectivamente.

## 4. Chats e mensagens

Prefixo: `/v1/chats`

  -------------------------------------------------------------------------------------------
  Método                  Endpoint                                    Descrição
  ----------------------- ------------------------------------------- -----------------------
  GET                     `/v1/chats/me`                              Lista os chats do
                                                                      usuário autenticado.

  GET                     `/v1/chats/{chatId}`                        Consulta um chat e seus
                                                                      membros.

  POST                    `/v1/chats`                                 Cria um chat.

  PATCH                   `/v1/chats/{chatId}`                        Atualiza o nome e a
                                                                      descrição do chat.

  DELETE                  `/v1/chats/{chatId}`                        Exclui um chat.

  POST                    `/v1/chats/{chatId}/member/{memberId}`      Adiciona um membro ao
                                                                      chat.

  DELETE                  `/v1/chats/{chatId}/member/{memberId}`      Remove um membro do
                                                                      chat.

  GET                     `/v1/chats/{chatId}/messages`               Lista as mensagens de
                                                                      um chat.

  POST                    `/v1/chats/{chatId}/messages`               Envia uma mensagem ao
                                                                      chat.

  GET                     `/v1/chats/{chatId}/messages/{messageId}`   Consulta uma mensagem
                                                                      específica.
  -------------------------------------------------------------------------------------------

### Criação de chat

O endpoint `POST /v1/chats` recebe um objeto `CreateChatDTO`:

``` json
{
  "chatTypeId": 1,
  "chatName": "Grupo de estudos",
  "chatDescription": "Estudos de programação",
  "membersId": [
    "00000000-0000-0000-0000-000000000000"
  ]
}
```

Os campos `chatName`, `chatDescription` e `membersId` são opcionais. O
`chatTypeId` é obrigatório.

Para atualizar um chat, utilize `PATCH /v1/chats/{chatId}`, enviando os
campos `chatName` e/ou `chatDescription`.

### Envio de mensagem

O endpoint `POST /v1/chats/{chatId}/messages` recebe:

``` json
{
  "content": "Olá! Vamos estudar juntos?"
}
```

O `chatId` é um número inteiro e o conteúdo da mensagem é enviado no
corpo da requisição.

As listagens de chats e mensagens aceitam `page` e `limit`, com valores
padrão de 1 e 10.

## 5. Health Check

  Método   Endpoint     Descrição
  -------- ------------ -------------------------------------
  GET      `/v1/ping`   Verifica se a API está respondendo.

A resposta implementada pelo controller é `Pong`.

## 6. Padrão de respostas

As respostas de sucesso seguem o modelo `SuccessResponse`, contendo:

``` json
{
  "success": true,
  "title": "Operação realizada corretamente.",
  "status": 200,
  "response": {
    "result": "Resultado da operação"
  }
}
```

O conteúdo de `response` varia conforme a operação. Por exemplo, pode
conter `jwt`, `user`, `users`, `post`, `posts`, `chat`, `chats`,
`message` ou `messages`.

Nas rotas paginadas, a resposta também pode conter:

``` json
{
  "pagination": {
    "page": 1,
    "limit": 10,
    "total_Elements": 100,
    "total_Pages": 10
  }
}
```

As respostas de erro utilizam o modelo `ErrorResponse`, com os campos
`success`, `title`, `detail` e `status`.

## 7. Observações sobre a implementação

-   **Paginação:** os controllers validam se `page` e `limit` são
    maiores que zero. Valores inválidos geram erro de requisição.
-   **Autenticação:** a API utiliza JWT para identificar o usuário
    autenticado. Algumas operações usam o identificador presente no
    token, sem exigir que ele seja enviado novamente no corpo da
    requisição.
-   **Status HTTP:** o Swagger documenta respostas `200 OK` para
    praticamente todas as operações. Entretanto, alguns controllers
    retornam `201 Created`, inclusive em operações de consulta e
    atualização.
-   **Consulta de mensagem:** a rota
    `GET /v1/chats/{chatId}/messages/{messageId}` está implementada como
    consulta de mensagem. Apesar de o método do controller possuir o
    nome `DeleteMessage`, ele não realiza uma exclusão.
-   **Exclusão de chat:** a rota `DELETE /v1/chats/{chatId}` existe, mas
    a documentação não especifica se a exclusão é lógica ou física.

A especificação OpenAPI pode ser consultada no [Swagger
JSON](https://sabires.onrender.com/swagger/v1/swagger.json). A descrição
das operações acima foi complementada pela implementação dos controllers
e dos DTOs presentes no repositório.

Esta documentação é um resumo das rotas disponíveis, não uma reprodução
integral do Swagger. Para conferir os modelos completos de requisição e
resposta, os códigos HTTP e os detalhes adicionais de cada operação,
consulte a [interface interativa do
Swagger](https://sabires.onrender.com/swagger/index.html).
