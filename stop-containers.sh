#!/bin/bash
 echo "=====================Stopping========================"
 docker stop ha
 docker stop grafana
 docker stop haw
 docker stop dc
 docker stop bc
 docker stop mosquitto
 docker stop influxdb
 docker stop rabbit
 docker stop logstash
 docker stop kibana
 docker stop elasticsearch