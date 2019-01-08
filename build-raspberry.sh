#!/bin/bash
 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.yaml up --build
 echo "=====================Stopping========================"
 docker stop mosquitto
 docker stop influxdb
 docker stop rabbit
 