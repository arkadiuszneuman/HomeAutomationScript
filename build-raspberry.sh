 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.yaml up --build
 echo ""=====================Stopping========================""
 docker stop homeassistant_rabbitmq_1
 docker stop homeassistant_mosquitto_1
 docker stop homeassistant_influxdb_1
 