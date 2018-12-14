 echo "=====================Starting========================"``
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.yaml -f ./docker-compose.windows.yaml up --build
 echo ""=====================Stopping========================""
 docker stop homeautomationscript_mosquitto_1
 docker stop homeautomationscript_influxdb_1
 docker stop homeautomationscript_rabbitmq_1
