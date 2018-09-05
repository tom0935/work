<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
<!-- Force latest IE rendering engine or ChromeFrame if installed -->
<!--[if IE]>
<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
<![endif]-->
<meta charset="utf-8">
<title>jQuery File Upload Demo</title>
<meta name="description" content="File Upload widget with multiple file selection, drag&amp;drop support, progress bars, validation and preview images, audio and video for jQuery. Supports cross-domain, chunked and resumable file uploads and client-side image resizing. Works with any server-side platform (PHP, Python, Ruby on Rails, Java, Node.js, Go etc.) that supports standard HTML form file uploads.">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<!-- Bootstrap styles -->
<link rel="stylesheet" href="~/Content/fileUpload/ext/bootstrap.min.css">
<!-- Generic page styles -->
<link rel="stylesheet" href="~/Content/fileUpload/css/style.css">
<!-- blueimp Gallery styles -->
<link rel="stylesheet" href="~/Content/fileUpload/ext/blueimp-gallery.min.css">
<!-- CSS to style the file input field as button and adjust the Bootstrap progress bars -->
<link rel="stylesheet" href="~/Content/fileUpload/css/jquery.fileupload.css">
<link rel="stylesheet" href="~/Content/fileUpload/css/jquery.fileupload-ui.css">
<!-- CSS adjustments for browsers with JavaScript disabled -->
<noscript><link rel="stylesheet" href="~/Content/fileUpload/css/jquery.fileupload-noscript.css"></noscript>
<noscript><link rel="stylesheet" href="~/Content/fileUpload/css/jquery.fileupload-ui-noscript.css"></noscript>
</head>
<body>

<div class="container">

    <!-- The file upload form used as target for the file upload widget -->
    <form id="fileupload" action="UploadProgress.aspx" method="POST" enctype="multipart/form-data">
        <!-- Redirect browsers with JavaScript disabled to the origin page -->
  <!--      <noscript><input type="hidden" name="redirect" value="http://blueimp.github.io/jQuery-File-Upload/"></noscript>-->
        <!-- The fileupload-buttonbar contains buttons to add/delete files and start/cancel the upload -->
        <div class="row fileupload-buttonbar">
            <div class="col-lg-7">
                <!-- The fileinput-button span is used to style the file input field as button -->
                <span class="btn btn-success fileinput-button">
                    <i class="glyphicon glyphicon-plus"></i>
                    <span>選擇檔案</span>
                    <input type="file" name="files[]" multiple>
                </span>
                <button type="submit" class="btn btn-primary start">
                    <i class="glyphicon glyphicon-upload"></i>
                    <span>檔案上傳</span>
                </button>
                <button type="reset" class="btn btn-warning cancel">
                    <i class="glyphicon glyphicon-ban-circle"></i>
                    <span>取消上傳</span>
                </button>
                <button type="button" class="btn btn-danger delete">
                    <i class="glyphicon glyphicon-trash"></i>
                    <span>刪除檔案</span>
                </button>
                <input type="checkbox" class="toggle">
                <!-- The global file processing state -->
                <span class="fileupload-process"></span>
            </div>
            <!-- The global progress state -->
            <div class="col-lg-5 fileupload-progress fade">
                <!-- The global progress bar -->
                <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100">
                    <div class="progress-bar progress-bar-success" style="width:0%;"></div>
                </div>
                <!-- The extended global progress state -->
                <div class="progress-extended">&nbsp;</div>
            </div>
        </div>
        <!-- The table listing the files available for upload/download -->
        <table role="presentation" class="table table-striped"><tbody class="files"></tbody></table>
    </form>
    <br>
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">上傳說明</h3>
        </div>
        <div class="panel-body">
            <ul>
                <li>上傳檔案大小限制在 <strong>50 MB</strong> 以下。</li>
                <li>限制上傳檔案類型： (<strong>JPG, GIF, PNG 及 MS Office 系列檔案</strong>) .</li> <!--上傳類型的限制設定在 main.js-->
            </ul>
        </div>
    </div>
</div>
<!-- The blueimp Gallery widget -->

<!-- The template to display files available for upload -->
    <script id="template-upload" type="text/x-tmpl">
       {% for (var i=0, file; file=o.files[i]; i++) { %}
        <tr class="template-upload fade">
            <td>
                <span class="preview"></span>
            </td>
            <td>
                <p class="name">{%=file.name%}</p>
                <strong class="error text-danger"></strong>
            </td>
            <td>
                <p class="size">Processing...</p>
                <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0"><div class="progress-bar progress-bar-success" style="width:0%;"></div></div>
            </td>
            <td>
                {% if (!i && !o.options.autoUpload) { %}
                <button class="btn btn-primary start" disabled>
                    <i class="glyphicon glyphicon-upload"></i>
                    <span>Start</span>
                </button>
                {% } %}
                {% if (!i) { %}
                <button class="btn btn-warning cancel">
                    <i class="glyphicon glyphicon-ban-circle"></i>
                    <span>Cancel</span>
                </button>
                {% } %}
            </td>
        </tr>
        {% } %}
    </script>
    <!-- The template to display files available for download -->
    <script id="template-download" type="text/x-tmpl">
        {% for (var i=0, file; file=o.files[i]; i++) { %}
        <tr class="template-download fade">
            <td>
                <span class="preview">
                    {% if (file.thumbnailUrl) { %}
                    <a href="{%=file.url%}" target="_blank" title="{%=file.name%}" download="{%=file.name%}" data-gallery><img src="{%=file.thumbnailUrl%}"></a>
                    {% } %}
                </span>
            </td>
            <td>
                <p class="name">
                    {% if (file.url) { %}
                    <a href="{%=file.url%}" target="_blank" title="{%=file.name%}" download="{%=file.name%}" {%=file.thumbnailUrl?'data-gallery':''%}>{%=file.name%}</a>
                    {% } else { %}
                    <span>{%=file.name%}</span>
                    {% } %}
                </p>
                {% if (file.error) { %}
                <div><span class="label label-danger">Error</span> {%=file.error%}</div>
                {% } %}
            </td>
            <td>
                <span class="size">{%=o.formatFileSize(file.size)%}</span>
            </td>
            <td>
                {% if (file.deleteUrl) { %}
                <button class="btn btn-danger delete" data-type="{%=file.deleteType%}" data-url="{%=file.deleteUrl%}" {% if (file.deletewithcredentials) { %} data-xhr-fields='{"withCredentials":true}' {% } %}>
                    <i class="glyphicon glyphicon-trash"></i>
                    <span>Delete</span>
                </button>
                <input type="checkbox" name="delete" value="1" class="toggle">                                                
               {% } else { %}><button class="btn btn-warning cancel">
                    <i class="glyphicon glyphicon-ban-circle"></i>
                    <span>Cancel</span>
                </button>
                {% } %}
            </td>
        </tr>
        {% } %}
    </script>
    <script src="~/Content/fileUpload/js/jquery_1.11.0.min.js"></script>
    <!-- The jQuery UI widget factory, can be omitted if jQuery UI is already included -->
   <!-- jquery.ui.widget.js 不能省略，否則已上傳及要上傳之清單會看不到-->
    <script src="~/Content/fileUpload/js/vendor/jquery.ui.widget.js"></script>
    <!-- The Templates plugin is included to render the upload/download listings -->
    <script src="~/Content/fileUpload/ext/tmpl.min.js"></script>
    <!-- The Load Image plugin is included for the preview images and image resizing functionality -->
    <script src="~/Content/fileUpload/ext/load-image.all.min.js"></script>
    <!-- The Canvas to Blob plugin is included for image resizing functionality -->
    <script src="~/Content/fileUpload/ext/canvas-to-blob.min.js"></script>
    <!-- Bootstrap JS is not required, but included for the responsive demo navigation -->
    <script src="~/Content/fileUpload/css/bootstrap-3.1.1-dist/js/bootstrap.min.js"></script>
    <!-- blueimp Gallery script -->
    <script src="~/Content/fileUpload/ext/jquery.blueimp-gallery.min.js"></script>
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="~/Content/fileUpload/js/jquery.iframe-transport.js"></script>
    <!-- The File Upload 基本檔案上傳套件 -->
    <script src="~/Content/fileUpload/js/jquery.fileupload.js"></script>
    <!-- The File Upload 檔案上傳處理套件 (processing plugin) -->
    <script src="~/Content/fileUpload/js/jquery.fileupload-process.js"></script>
    <!-- The File Upload 檔案上傳圖片檔預覽套件 (image preview & resize plugin) -->
    <script src="~/Content/fileUpload/js/jquery.fileupload-image.js"></script>
    <!-- The File Upload 檔案上傳音樂檔預覽套件 (audio preview plugin)  若不允許上傳影片，可以不用載入-->
    <script src="~/Content/fileUpload/js/jquery.fileupload-audio.js"></script>
    <!-- The File Upload 檔案上傳影片預覽套件 (video preview plugin) 若不允許上傳影片，可以不用載入 -->
    <script src="~/Content/fileUpload/js/jquery.fileupload-video.js"></script>
    <!-- The File Upload 檔案上傳驗證套件 (validation plugin) -->
    <script src="~/Content/fileUpload/js/jquery.fileupload-validate.js"></script>
    <!-- The File Upload 檔案上傳使用者介面套件 (user interface plugin) -->
    <script src="~/Content/fileUpload/js/jquery.fileupload-ui.js"></script>
    <script>        var fileuploadurl = "UploadProgress.aspx";</script>
    <script src="~/Content/fileUpload/js/main.js"></script>  


</body>
</html>


