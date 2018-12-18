# The domain of your component. Should be equal to the name of your component.
import logging, time, hmac, hashlib, random, base64, json, socket

from homeassistant.components.switch import SwitchDevice
from datetime import timedelta
from homeassistant.util import Throttle
from homeassistant.components.switch import DOMAIN
# from homeassistant.components.sonoff import (DOMAIN, SonoffDevice)
from custom_components.sonoff import (DOMAIN as SONOFF_DOMAIN, SonoffDevice)

# @TODO add PLATFORM_SCHEMA here (maybe)

SCAN_INTERVAL = timedelta(seconds=10)

_LOGGER = logging.getLogger(__name__)

async def async_setup_platform(hass, config, async_add_entities, discovery_info=None):
    """Add the Sonoff Switch entities"""
 
    entities = []

    for device in hass.data[SONOFF_DOMAIN].get_devices(force_update = True):
        # the device has multiple switches, split them by outlet
        if 'switches' in device['params']:
            for outlet in device['params']['switches']:
                entity = SonoffSwitch(hass, device, outlet['outlet'])
                entities.append(entity)
        
        # normal device = Sonoff Basic (and alike)
        else:
            entity = SonoffSwitch(hass, device)
            entities.append(entity)    

    async_add_entities(entities, update_before_add=False)

class SonoffSwitch(SonoffDevice, SwitchDevice):
    """Representation of a Sonoff device (switch)."""

    def __init__(self, hass, device, outlet = None):
        """Initialize the device."""

        # add switch unique stuff here if needed
        SonoffDevice.__init__(self, hass, device, outlet)

    # entity id is required if the name use other characters not in ascii
    @property
    def entity_id(self):
        """Return the unique id of the switch."""
        return "{}.{}_{}".format(DOMAIN, SONOFF_DOMAIN, self._deviceid)