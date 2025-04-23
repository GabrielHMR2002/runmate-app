@echo off
echo ========================================
echo Iniciando a solucao RunMate em Docker
echo ========================================
echo.

echo Parando conteineres anteriores se existirem...
docker-compose down

echo.
echo Construindo imagens Docker...
docker-compose build

echo.
echo Iniciando os servicos...
docker-compose up -d

echo.
echo Aguardando a inicializacao dos servicos...
timeout /t 10 /nobreak

echo.
echo ========================================
echo Solucao iniciada com sucesso!
echo.
echo Authentication Service: http://localhost:5001/swagger
echo Engagement Service: http://localhost:5202/swagger
echo RabbitMQ Management: http://localhost:15672/ (guest/guest)
echo ========================================