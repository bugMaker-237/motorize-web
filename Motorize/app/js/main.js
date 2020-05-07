// mozfullscreenerror event handler
function errorHandler()
{
  alert('mozfullscreenerror');
}
document.addEventListener('mozfullscreenerror', errorHandler, false);

/**
 * toggle full screen
 * @param {HTMLElement} element
 */
function toggleFullScreen(element)
{
  var fullscreenElement = document.fullscreenElement ||
    document.mozFullScreenElement || document.webkitFullscreenElement;
  var justDisable = element === fullscreenElement;
  if (fullscreenElement)
  {  // current working methods
    if (document.exitFullscreen)
    {
      document.exitFullscreen();
    } else if (document.mozCancelFullScreen)
    {
      document.mozCancelFullScreen();
    } else if (document.webkitCancelFullScreen)
    {
      document.webkitCancelFullScreen();
    }
    
  } 

  if (!justDisable && element)
  {
    if (element.requestFullscreen)
    {
      element.requestFullscreen();
    } else if (element.mozRequestFullScreen)
    {
      element.mozRequestFullScreen();
    } else if (element.webkitRequestFullscreen)
    {
      element.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
    }
  }
}

function exportCanvasAsPNG(canvas)
{
  var image = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream"); //Convert image to 'octet-stream' (Just a download, really)
  window.location.href = image;
}

var dataStorage = {
  key: 'graph-datas',
  getAll: function ()
  {
    return localStorage.getItem(this.key) || [];
  },
  deleteAll: function ()
  {
    localStorage.removeItem(this.key);
  },
  add: function (data)
  {
    var graphData = this.getAll() || [];
    graphData.push(data);
    localStorage.setItem(this.key, graphData);
  },
  delete: function (dataId)
  {
    var graphData = this.getAll() || [];
    graphData = graphData.filter(function (val)
    {
      return val.Id !== dataId;
    });
    this.localStorage.setItem(this.key, graphData);
  }

}