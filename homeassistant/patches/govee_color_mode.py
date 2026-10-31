"""Re-apply the color_mode fix to the HACS "govee" integration (LaggAt/hacs-govee).

Upstream 2025.7.1 never reports color_mode, which HA requires: every state
write fails with "does not report a color mode" (thousands of log errors,
failing light.turn_on calls from Node-RED). A HACS update overwrites the file,
so packages/govee.yaml runs this at startup and after an update.

Prints "patched" when it changed the file, "ok" when the fix is already there
(or upstream fixed it), "missing" when the integration isn't installed.
"""
import pathlib

LIGHT = pathlib.Path("/config/custom_components/govee/light.py")
ANCHOR = "    async def async_turn_on(self, **kwargs):"
FIX = '''    @property
    def color_mode(self) -> ColorMode:
        """Current color mode (HA requires it; upstream 2025.7.1 never set it)."""
        modes = self.supported_color_modes
        if ColorMode.COLOR_TEMP in modes and self._device.color_temp:
            return ColorMode.COLOR_TEMP
        if ColorMode.HS in modes:
            return ColorMode.HS
        return next(iter(modes))

    @property
    def color_temp_kelvin(self):
        """Return the color temperature in Kelvin (0 from Govee means "not in white mode")."""
        return self._device.color_temp or None

'''

if not LIGHT.exists():
    print("missing")
else:
    source = LIGHT.read_text()
    if "def color_mode" in source or ANCHOR not in source:
        print("ok")
    else:
        LIGHT.write_text(source.replace(ANCHOR, FIX + ANCHOR, 1))
        print("patched")
