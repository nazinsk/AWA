var express = require('express');
var app = express();

var mongoose = require('mongoose');
// MONGODB ============================================
// conectando ao mongodb no localhost, criando o banco de dados contato
//mongoose.connect('mongodb://teste.koezu.com/testes');
mongoose.connect('mongodb://teste:teste@ds011830.mlab.com:11830/heroku_fn83l288');


var Teste = mongoose.model('Teste', { nome: String, data: { type: Date, default: Date.now }});


var bodyParser = require('body-parser');

app.use(bodyParser.json()); // for parsing application/json
//app.use(bodyParser.urlencoded({ extended: true })); // for parsing application/x-www-form-urlencoded
//app.use(bodyParser.json({ type: 'application/vnd.api+json' }));

// // parse application/json
// app.use(bodyParser.json());
// // parse application/vnd.api+json as json


app.get('/api/testes', function (req, res) {
  Teste.find(function(err, testes) {
    // Em caso de erros, envia o erro na resposta
    if (err)
        res.send(err)
    // Retorna todos os contatos encontrados no BD
    res.json(testes);
  });

});

app.post('/api/testes', function (req, res) {

  console.log(req.body);
  //res.send('sim');
  Teste.create({
      nome : req.body.nome
    }, function(err, teste) {
      if (err)
        res.send(err);
      // Busca novamente todos os contatos após termos inserido um novo registro
      Teste.find(function(err, testes) {
        if (err)
          res.send(err)
        res.json(testes);
      });
    });
});

app.listen(3000, function () {
  console.log('Example app listening on port 3000!');
});
