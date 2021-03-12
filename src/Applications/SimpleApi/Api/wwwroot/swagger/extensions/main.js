//封装方法
if (!window.location.request) {
    /**
     *
     * 获取URL地址栏参数(/a/b/c?xx=xx&a2=xx&a3=xx) 
     * LCTR 2018-03-21
     *
     * @method request
     * 
     * @param {string} param 参数名称
     *
     */
    window.location.request = function (param) {    /*获取URL的字符串*/    var sSource = String(window.document.location); var sName = param; var sReturn = ""; var sQUS = "?"; var sAMP = "&"; var sEQ = "="; var iPos;    /*获取sSource中的"?"，无则返回 -1*/    iPos = sSource.indexOf(sQUS); if (iPos == -1) return;    /*汲取参数，从iPos位置到sSource.length-iPos的位置，若iPos = -1，则：从-1 到 sSource.length+1  */    var strQuery = sSource.substr(iPos, sSource.length - iPos);    /* alert(strQuery);先全部转换为小写 */    var strLCQuery = strQuery.toLowerCase(); var strLCName = sName.toLowerCase();    /*从子字符串strLCQuery中查找“?”、参数名，以及“=”，即“?参数名=”  */    iPos = strLCQuery.indexOf(sQUS + strLCName + sEQ);    /*如果不存在*/    if (iPos == -1) {        /*继续查找可能的后一个参数，即带“&参数名=”*/        iPos = strLCQuery.indexOf(sAMP + strLCName + sEQ); }    /*判断是否存在参数 */    if (iPos != -1) { sReturn = strQuery.substr(iPos + sName.length + 2, strQuery.length - (iPos + sName.length + 2)); var iPosAMP = sReturn.indexOf(sAMP); if (iPosAMP == -1) { return sReturn; } else { sReturn = sReturn.substr(0, iPosAMP); } } return sReturn; }
}

if (!String.prototype.getParam) {
    /**
     *
     * 获取字符串参数(?xx=xx&yy=yy) 
     * LCTR 2018-05-02
     *
     * @method getParam
     * 
     * @param {string} param 参数名称
     *
     */
    String.prototype.getParam = function (param) { var query = this; var iLen = param.length; var iStart = query.indexOf(param); if (iStart == -1) return ""; iStart += iLen + 1; var iEnd = query.indexOf("&", iStart); if (iEnd == -1) return query.substring(iStart); return query.substring(iStart, iEnd); }
}

if (!String.prototype.setParam) {
    /**
     *
     * 设置url参数(?xx=xx&yy=yy) 
     * LCTR 2018-05-03
     *
     * @method setParam
     * 
     * @param {string} param 参数名称
     * @param {string} value 值
     *
     */
    String.prototype.setParam = function (param, value) { var query = this; var iLen = param.length; var iStart = query.indexOf(param + '='); if (iStart == -1) return query += (query.indexOf('?') === -1 ? '?' : (query[query.length - 1] != '&' ? '&' : '')) + param + '=' + value; iStart += iLen + 1; var iEnd = query.indexOf("&", iStart); if (iEnd == -1) return query.substring(0, iStart) + value; return query.substring(0, iStart) + value + query.substring(iEnd, query.length); }
}

if (!String.prototype.setvalue2string) {
    /**
     *
     * 设置拼接字符串指定位置的值
     * LCTR 2018-12-14
     *
     * @method setvalue2string
     * 
     *
     * @param {any} value 值
     * @param {any} index 位置(索引,从1开始)
     * @param {any} char 拼接字符(默认逗号,)
     *
     */
    String.prototype.setvalue2string = function (value, index, char) { var query = this, _index = -1, start_index = 0, end_index = 0; char = typeof char == 'undefined' ? ',' : char; for (var i = 0; i < index; i++) { _index = query.indexOf(char, start_index); if (_index == -1) { query += char; _index = query.indexOf(char, start_index); if (i == index - 1) query = query.substring(0, query.length - 1); } if (i == index - 1) { end_index = _index; } else { start_index = _index + 1; } } return query.substring(0, start_index) + value + query.substring(end_index); }
}

if (!String.prototype.removevalue4string) {
    /**
     *
     * 删除拼接字符串指定位置的值
     * LCTR 2019-03-25
     *
     * @method removevalue4string
     *
     *
     * @param {any} index 位置(索引,从1开始)
     * @param {any} char 拼接字符(默认逗号,)
     *
     */
    String.prototype.removevalue4string = function (index, char) { var query = this; var _query = query.split(char || ','); _query.splice(index - 1, 1); return _query.join(char || ','); }
}

if (!String.prototype.IsGUID) {
    /**
     *
     * 是否为GUID
     * LCTR 2019-01-11
     *
     * @method isGUID
     * 
     * @param {string} type 格式(默认为由连字符分隔的32位字符)
     * 
     * @returns {boolean} 是/否
     *
     */
    String.prototype.IsGUID = function (type) { var value = this || ''; type == type || ''; type == 'B' || type == 'P' || type == 'X' ? (value = value.substring(1, value.length - 1)) : 1; var _array = type == 'N' ? [] : (type == 'X' ? value.split(',') : value.split('-')); return type == 'N' ? value.length == 32 : (type == 'X' ? value.length == 66 && _array.length == 10 && _array[0][1] == 'x' && _array[1][1] == 'x' && _array[2][1] == 'x' && _array[3][2] == 'x' && _array[4][1] == 'x' && _array[5][1] == 'x' && _array[6][1] == 'x' && _array[7][1] == 'x' && _array[8][1] == 'x' && _array[9][1] == 'x' && _array[10][1] == 'x' : (value.length == 36 && _array.length == 5 && _array[0].length == 8 && _array[1].length == 4 && _array[2].length == 4 && _array[3].length == 4 && _array[4].length == 12)); }
}

if (!String.prototype.group) {
    /**
     *
     * 字符串分组
     * LCTR 2019-01-15
     *
     * @method group
     * 
     * @param {any} x 长度
     * @param {string} c 标志
     * @param {boolean} r 是否逆向
     * 
     * @returns {string} 结果
     *
     */
    String.prototype.group = function (x, c, r) { x = typeof x == 'undefined' || x == null || x == '' ? 4 : x; c = typeof c == 'undefined' || c == null ? ',' : c; var value = this || ''; var decimal = ''; value.indexOf('.') < 0 ? 1 : (c == '.' ? (c = ',') : 1, decimal = '.' + value.split('.')[1], value = value.split('.')[0]); return value.split('').map(function (item, index) { if (r) { return index + 1 != value.length && (value.length - index) % (x * 1 + 1) == 0 && item == c ? '' : item; } else { return index + 1 != value.length && (value.length - 1 - index) % x == 0 ? (item + c) : item; } }).join('') + decimal; }
}

//拓展String的toBlob方法，将base64字符串转为blob对象
if (!String.prototype.toBlob) {
    String.prototype.toBlob = function (contentType, sliceSize) {
        var b64Data = this;
        contentType = contentType || '';
        sliceSize = sliceSize || 512;

        var byteCharacters = atob(b64Data);
        var byteArrays = [];

        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            var slice = byteCharacters.slice(offset, offset + sliceSize);

            var byteNumbers = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            var byteArray = new Uint8Array(byteNumbers);

            byteArrays.push(byteArray);
        }

        var blob = new Blob(byteArrays, { type: contentType });
        return blob;
    };
}

//拓展String的toDate方法，将日期字符串转为日期
if (!String.prototype.toDate) {
    String.prototype.toDate = function () {
        var str = this;
        str = str.replace(/-/g, "/");
        return new Date(str);
    };
}
if (!String.prototype.contains) {
    String.prototype.contains = function (subStr) {
        return this.indexOf(subStr) > -1;
    };
}

if (!String.prototype.isIdentityCard) {
    /**
     *
     * 验证身份证号码
     * LCTR 2018-09-10
     *
     * @method isIdentityCard
     * 
     * @param {string} birthFormat  出生日期格式化字符串
     * 
     * @returns {object} 成功则返回身份信息,失败则返回false
     */
    String.prototype.isIdentityCard = function (birthFormat) { var that = this, info = {}, address = '11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91', checkAddress = () => { return address.indexOf(that.substr(0, 2)) > -1; }, checkBirth = (length) => { return info.birth = new Date(that.substr(6, length).setvalue2string('-', length - 3, '').setvalue2string('-', length, '')), info.birth == 'Invalid Date' ? !1 : (birthFormat ? info.birth = info.birth.format(birthFormat) : 1, true); }, getSex = () => { info.sex = parseInt(that.substr(-4, 3)) % 2 == 0 ? '女' : '男'; }; return that ? (that.length == 15 ? (isNaN(that) || parseInt(that) < Math.pow(10, 14) ? !1 : (!checkAddress() ? !1 : (!checkBirth(6) ? !1 : (getSex(), info)))) : (that.length == 18 ? (isNaN(that.substr(0, 17)) || parseInt(that.substr(0, 17)) < Math.pow(10, 16) || isNaN(that.substr(-1).replace(/[xX]/g, '0')) ? !1 : (!checkAddress() ? !1 : (!checkBirth(8) ? !1 : (getSex(), info)))) : !1)) : !1; }
}

if (!String.prototype.trimEmpty) {
    /*LCTR 2018-11-16*/
    /**
     *
     * 去除字符串中的空格
     * LCTR 2018-09-10
     *
     * @method trimEmpty
     *
     */
    String.prototype.trimEmpty = function () { if (arguments.length == 0) return this; return this.replace(/^\s+|\s+$/gm, ''); }
}

if (!String.prototype.format) {
    /**
     *
     * 字符串占位符形式实现
     * 使用示例：
     *           ①"abc{0}def{1}ghi{2}jk".format("a","b","c")
     *           ②"a{name}b{sex},c{age}".format({name: "A",sex: "B",age: C});
     * LCTR 2018-03-21
     *
     * @method format
     *
     */
    String.prototype.format = function () { if (arguments.length == 0) return this; var param = arguments[0]; var s = this; if (typeof (param) == 'object') { for (var key in param) { s = s.replace(new RegExp("\\{" + key + "\\}", "g"), typeof param[key] == 'undefined' || param[key] == null ? '' : param[key]); } return s; } else { for (var i = 0; i < arguments.length; i++) { s = s.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i] || ''); } return s; } }
}

if (!Date.prototype.format) {
    /**
     * 
     * 格式化日期
     * 
     * @method  format
     * 
     * @param {string} format  格式(例如yyyy-MM-dd HH:mm:ss)
     * 
     * @returns {object} 值
     */
    Date.prototype.format = function (format) { var o = { "M+": this.getMonth() + 1, "d+": this.getDate(), "H+": this.getHours(), "h+": this.getHours(), "m+": this.getMinutes(), "s+": this.getSeconds(), "q+": Math.floor((this.getMonth() + 3) / 3), "S": this.getMilliseconds() }; if (/(y+)/i.test(format)) { format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length)); } for (var k in o) { if (new RegExp("(" + k + ")").test(format)) { format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length)); } } return format; }

    Date.prototype.addDays = function (d) {
        this.setDate(this.getDate() + d);
    };
    Date.prototype.addWeeks = function (w) {
        this.addDays(w * 7);
    };
    Date.prototype.addMonths = function (m) {
        var d = this.getDate();
        this.setMonth(this.getMonth() + m);
        if (this.getDate() < d)
            this.setDate(0);
    };
    Date.prototype.addYears = function (y) {
        var m = this.getMonth();
        this.setFullYear(this.getFullYear() + y);
        if (m < this.getMonth()) {
            this.setDate(0);
        }
    };
}

if (!Date.prototype.format_other) {
    //日期格式化
    Date.prototype.format_other = function (fmt) { //author: meizz 
        var o = {
            "M+": this.getMonth() + 1, //月份 
            "d+": this.getDate(), //日 
            "h+": this.getHours(), //小时 
            "m+": this.getMinutes(), //分 
            "s+": this.getSeconds(), //秒 
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
            "S": this.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    };
}

//拓展Array的forEach方法，用在某些浏览器没有forEach方法
if (!Array.prototype.forEach) {
    Object.defineProperty(Array.prototype, "forEach", {
        value: function (callback) {
            var d = this || [];
            if (!callback) return;

            for (var i = 0; i < d.length; i++) {
                var elem = d[i];
                callback(elem, i);
            }
        },
        enumerable: false
    });
}

if (!Array.prototype.indexOf) {
    //拓展Array的indexOf方法
    Object.defineProperty(Array.prototype, "indexOf", {
        value: function (val) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] == val) return i;
            }
            return -1;
        },
        enumerable: false
    });
}

if (!Array.prototype.exists) {
    //拓展Array的exists方法
    Object.defineProperty(Array.prototype, "exists", {
        value: function (val) {
            return this.indexOf(val) > -1;
        },
        enumerable: false
    });
}

if (!Array.prototype.removeItem) {
    //拓展Array的removeItem方法
    Object.defineProperty(Array.prototype, "removeItem", {
        value: function (val) {
            var index = this.indexOf(val);
            if (index > -1) {
                this.splice(index, 1);
            }
        },
        enumerable: false
    });
}

//确保开始时间小于结束时间
if (!window.checkStartEndDate) {
    window.checkStartEndDate = function (startDateId, endDateId) {
        var startDate = $("#" + startDateId).val();
        var endDate = $("#" + endDateId).val();

        var _startDate = new Date(startDate);
        var _endDate = new Date(endDate);
        if (_startDate > _endDate) {
            dialogMsg("请输入选择有效的时间（结束时间必须大于或者等于开始时间！）");
            return false;
        }
        else {
            return true;
        }
    }
}

/*jQuery拓展，绑定input file并转为base64字符串
 *使用示例:
    $('#img').bindImgBase64(function (base64) {
        $('#img-display').attr('src', base64);
    });
 */
if (!$.prototype.bindImgBase64) {
    $.prototype.bindImgBase64 = function (callback) {
        var _callback = callback || function () { };
        var thisElement = this[0];
        thisElement.onchange = function () {
            var img = event.target.files[0];

            // 判断是否图片
            if (!img) {
                return;
            }
            // 判断图片格式
            if (!(img.type.indexOf('image') == 0 && img.type && /\.(?:jpg|jpeg|png|gif|bmp)$/i.test(img.name))) {
                throw '图片只能是jpg,jpeg,gif,png,bmp';
            }
            var reader = new FileReader();
            reader.readAsDataURL(img);
            reader.onload = function (e) {
                var imgBase64 = e.target.result;
                $(thisElement).attr('base64', imgBase64);
                _callback(imgBase64);
            }
        };
    };
    $.prototype.getImgBase64 = function () {
        return this.attr('base64');
    };
}

//拓展FileReader的readAsBinaryString方法
if (!FileReader.prototype.readAsBinaryString) {
    FileReader.prototype.readAsBinaryString = function (fileData) {
        var binary = "";
        var pt = this;
        var reader = new FileReader();
        reader.onload = function (e) {
            var bytes = new Uint8Array(reader.result);
            var length = bytes.byteLength;
            for (var i = 0; i < length; i++)
                binary += String.fromCharCode(bytes[i]);
        }
        pt.content = binary;
        $(pt).trigger('onload');
        reader.readAsArrayBuffer(fileData);
    }
}

//拓展文件操作，将文件转为base64
//回调参数为文件base64内容和文件名
if (!$.prototype.getFileBase64) {
    $.prototype.getFileBase64 = function (callBack) {
        var _callBack = callBack || function () { };
        var file = $(this)[0].files[0];
        var reader = new FileReader();
        reader.readAsBinaryString(file);
        reader.onload = function (e) {
            var bytes = e.target.result;
            var base64 = btoa(bytes);
            var fileName = file.name;
            callBack(base64, fileName);
        }
    };
}

//获取元素中所有没有被disabled的name集合
if (!$.prototype.getNames) {
    $.prototype.getNames = function () {
        var nameList = [];
        $(this).find('[name]').not('[disabled]').each(function (index, element) {
            var name = $(element).attr('name');
            nameList.push(name);
        });

        return nameList;
    };
}

//取表单域的所有值
if (!$.fn.getValues) {
    $.fn.getValues = function (options) {
        var defaults = {
            checkbox: 'array',
            not: []
        };
        var $container = $(this);

        options = $.extend({}, defaults, options);

        function getValues() {
            var values = {}, ckbFields = [];
            $container.find(":input[name]").each(function () {
                var that = $(this),
                    type = (that.attr("type") || '').toLowerCase(),
                    name = that.attr("name");
                if (type == 'button' || type == 'submit') return true;
                if (!name.length) return true;
                if ($.inArray(name, options.not) >= 0) return true;

                if (type == "checkbox") {
                    if (values[name] != undefined) {
                        return true;
                    }
                    ckbFields.push(name);
                    values[name] = getCheckboxValues(name);
                } else if (type == "radio") {
                    if (values[name] != undefined) {
                        return true;
                    }
                    values[name] = getRadioValue(name);
                } else {
                    var value = that.val();
                    //if (!value.length) return true;

                    if (values[name] == undefined) {
                        values[name] = value;
                    } else {
                        var arr = [];
                        arr.push(values[name]);
                        arr.push(value);

                        values[name] = arr.join(',');
                    }
                }
            });

            if (options.checkbox == 'string') {
                $.each(ckbFields, function (i, v) {
                    if ($.isArray(values[v])) values[v] = values[v].join(',');
                });
            }

            return values;
        }

        function getRadioValue(name) {
            return $container.find("input:radio[name='" + name + "']").val();
        }

        function getCheckboxValues(name) {
            return $.map($container.find("input:checkbox[name='" + name + "']:checked"), function (n) {
                return n.value;
            });
        }

        return getValues();
    };
}

//使用文件base64下载文件
if (!window.downloadFileBase64) {
    window.downloadFileBase64 = function (base64, fileName) {
        var blob = base64.toBlob();
        var reader = new FileReader();
        reader.readAsDataURL(blob);
        reader.onload = function (e) {
            // 转换完成，创建一个a标签用于下载
            var a = document.createElement('a');
            a.hidden = true;
            a.download = fileName;
            a.href = e.target.result;
            $("body").append(a);  // 修复firefox中无法触发click
            a.click();
            $(a).remove();
        }
    };
}

//使用文件Url下载文件
if (!window.downloadFile) {
    window.downloadFile = function (url) {
        var a = document.createElement('a');
        a.hidden = true;
        a.download = '';
        a.href = url
        $("body").append(a);  // 修复firefox中无法触发click
        a.click();
        $(a).remove();
    };
}

//拓展window的toDateString方法
if (!window.toDateString) {
    window.toDateString = function (date, fmt) {
        if (date) {
            return date.toDate().format(fmt);
        } else {
            return '';
        }
    };
}

//拓展window的getType方法
if (!window.getType) {
    window.getType = function (obj) {
        var type = typeof (obj);
        if (type == 'object') {
            type = Object.prototype.toString.call(obj);
            if (type == '[object Array]') {
                return 'array';
            } else if (type == '[object Object]') {
                return "object";
            } else {
                return 'param is no object type';
            }
        } else {
            return type;
        }
    }
}

if (!window.delayedEvent) {
    var delayedEvents = {};

    /**
     *
     * 延时事件 
     * LCTR 2020-12-08
     *
     * @method delayedEvent
     *
     * @param {Function} handler 处理函数
     * @param {number} event 延时(毫秒)(默认800)
     * @param {string} event 事件名称
     * @param {boolean} repeat 禁止重复(默认禁止)
     *
    */
    window.delayedEvent = function (handler, timeout, event, repeat = false) {
        if (!repeat) {
            event ? 1 : event = Date.now();
            if (delayedEvents[event])
                window.clearTimeout(delayedEvents[event]);
        }

        delayedEvents[event] = window.setTimeout(() => { delayedEvents[event] = 0; handler(); }, timeout || 800);
    }
}

if (!window.showDialog) {
    /**
    *
    * 展示对话框
    * LCTR 2020-12-08
    *
    * @method showDialog
    *
    * @param {string} title 标题
    * @param {Array<Array<string>>} content 内容
    * [
    *   ['H5', '标题', '内容']},
    *   ['input', '标题', '内容']},
    *   ['input-readonly', '标题', '内容']},
    *   ['label', '标题', '内容']}, 
    *   ['label', '内容']}
    * ]
    * @param {any} button 操作按钮
    * {
    *   '文本': {
    *       'click': ()=>{ },
    *       'dblclick': ()=>{ },
    *   },
    *   '文本': {
    *       'click': ()=>{ }
    *   },
    *   '文本': [
    *       '提示',{
    *           'click': ()=>{ }
    *       }
    *   ]
    * }
    * @param {any} closeButton 显示关闭按钮(默认显示)
    *
    */
    window.showDialog = (title, content, button, closeButton = true) => {
        var close = () => { body.fadeOut(); },
            body = $('<div id="dialog" class="dialog-ux"><div class="backdrop-ux"></div><div class="modal-ux"><div class="modal-dialog-ux"><div class="modal-ux-inner"><div class="modal-ux-header"><h3>' + title + '</h3></div><div class="modal-ux-content"><div class="auth-container"><div><div><div class="auth-container-items"></div><div class="auth-btn-wrapper"></div></div></div></div></div></div></div></div></div></div>'),
            info = '';

        if (button)
            $.each(button, (key, value) => {
                var title = $.isArray(value) ? value[0] : '',
                    text = key,
                    events = $.isArray(value) ? value[1] : value,
                    btn = $('<button class="btn modal-btn casBtn auth authorize button" title="' + title + '">' + text + '</button>');

                $.each(events, (type, event) => {
                    btn.on(type, event);
                });

                btn.appendTo(body.find('.auth-btn-wrapper'));
            });

        if (content)
            $.each(content, (index, item) => {
                var type = item[0],
                    _title = item.length > 2 ? item[1] : null,
                    _content = item.length > 2 ? item[2] : item[1],
                    _key = 'key_' + index;

                if (_title)
                    info += '<label for="' + _key + '">' + _title + ':</label>';

                switch (type) {
                    case 'H5':
                        info += '<h5>' + _content + '</h5>';
                        break;
                    case 'input':
                    case 'input-readonly':
                        info += '<section class="block-tablet col-10-tablet block-desktop col-10-desktop"><input type="text" ' + (type == 'input-readonly' ? 'readonly="readonly"' : '') + ' class="casInput" id="' + _key + '" data-name="' + _key + '" value="' + _content + '"></section>';
                        break;
                    case 'label':
                    default:
                        info += '<section class="block-tablet col-10-tablet block-desktop col-10-desktop"><label class="casInput" id="' + _key + '" data-name="' + _key + '">' + _content + '</label></section>';
                        break;
                }
            });

        if (closeButton) {
            $('<button class="btn modal-btn casBtn auth btn-done button">关闭</button>')
                .on('click', close)
                .appendTo(body.find('.auth-btn-wrapper'));
            $('<button class="close-modal"><svg width="20" height="20"><use href="#close" xlink:href="#close"></use></svg></button>')
                .on('click', close)
                .appendTo(body.find('.modal-ux-header'));
        }

        body.find('.auth-container-items').append(info);

        body.fadeIn().appendTo($('.scheme-container'));
    };
}

if (!window.addPlugIn) {
    /**
     * 添加插件
     * 
     * @param {string} name 名称
     * @param {Function} fun 回调方法
     * @param {string} svg 图标
     */
    window.addPlugIn = (name, fun, svg) => {
        var body = $('#plugInBody');
        if (body.length == 0)
            body = $('<div id="plugInBody" style="position:fixed; top:0px; right:0px; height:100%; /*background-color: #000;*/ z-index: 998;"></div>')
                .appendTo($('body'))
                .draggable({ containment: $('body'), scroll: false });

        $('<i title="{name}" class="plug-in ui-draggable ui-draggable-handle" style="position:absolute; right:5px; bottom:100px; z-index: 999;">{svg}</i>'.format({ name: name, svg: svg }))
            .animate({ bottom: (100 + ($('.plug-in').length * 60)) + 'px' })
            .appendTo(body)
            .on('click', (e) => { fun(name, e); });
    }
}

var callback = function () {
    //本土化
    var replacePlaceholder = setInterval(() => { $('.operation-filter-input').length ? ($('.operation-filter-input').attr('placeholder', '标签名称（区分大小写）'), window.clearInterval(replacePlaceholder)) : 0; });
};

document.readyState === "complete" || (document.readyState !== "loading" && !document.documentElement.doScroll) ? callback() : document.addEventListener("DOMContentLoaded", callback);