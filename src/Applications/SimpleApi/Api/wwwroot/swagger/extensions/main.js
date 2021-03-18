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
    * @param {boolean} closeButton 显示关闭按钮(默认显示)
    * @param {boolean} mask 显示遮罩(默认显示)
    * @param {boolean} drag 允许拖动(默认禁止)
    *
    */
    window.showDialog = (title, content, button, closeButton = true, mask = true, drag = false) => {
        var close = () => { $body.fadeOut(() => { $body.remove(); }); },
            $body = $('<div id="dialog" class="dialog-ux ' + (mask ? '' : 'dialog-ux-none') + '">' + (mask ? '<div class="backdrop-ux"></div>' : '') + '<div class="modal-ux"><div class="modal-dialog-ux"><div class="modal-ux-inner"><div class="modal-ux-header"><h3>' + title + '</h3></div><div class="modal-ux-content"><div class="auth-container"><div><div><div class="auth-container-items"></div><div class="auth-btn-wrapper"></div></div></div></div></div></div></div></div></div></div>'),
            info = '';

        if (button)
            $.each(button, (key, value) => {
                var title = $.isArray(value) ? value[0] : '',
                    text = key,
                    events = $.isArray(value) ? value[1] : value,
                    btn = $('<button class="btn swBtn authorize" title="' + title + '">' + text + '</button>');

                $.each(events, (type, event) => {
                    btn.on(type, event);
                });

                btn.appendTo($body.find('.auth-btn-wrapper'));
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
                    case 'password':
                        info += '<section class="block-tablet col-10-tablet block-desktop col-10-desktop"><input type="' + (type == 'password' ? 'password' : 'text') + '" ' + (type == 'input-readonly' ? 'readonly="readonly"' : '') + ' class="swInput" id="' + _key + '" data-name="' + _key + '" value="' + _content + '"></section>';
                        break;
                    case 'iframe':
                        info += '<iframe class="swIframe" id="' + _key + '" data-name="' + _key + '" src="' + _content + '" name="' + _title + '"><p>您的浏览器不支持iframes标签.</p></iframe >';
                        break;
                    case 'label':
                    default:
                        info += '<section class="block-tablet col-10-tablet block-desktop col-10-desktop"><label class="swLabel" id="' + _key + '" data-name="' + _key + '">' + _content + '</label></section>';
                        break;
                }
            });

        if (closeButton) {
            $('<button class="btn btn-done">关闭</button>')
                .on('click', close)
                .appendTo($body.find('.auth-btn-wrapper'));
            $('<button class="close-modal"><svg width="20" height="20"><use href="#close" xlink:href="#close"></use></svg></button>')
                .on('click', close)
                .appendTo($body.find('.modal-ux-header'));
        }

        $body.find('.auth-container-items').append(info);

        $body.fadeIn().appendTo($('.scheme-container'));

        if (drag)
            $body.find('.modal-ux')
                .draggable({ handle: ".modal-ux-header", scroll: false })
                .find('.modal-ux-header')
                .css({ 'cursor': 'move' });
    };
}

if (!window.addPlugIn) {
    /**
     * 添加插件
     * 
     * @param {string} name 名称
     * @param {string} title 标题
     * @param {Function} fun 回调方法
     * @param {string} svg 图标
     */
    window.addPlugIn = (name, title, fun, svg) => {
        var $body = $('#plugInBody');
        if ($body.length == 0) {
            $body = $('<div id="plugInBody"></div>')
                .appendTo($('body'))
                .draggable({ containment: $('#swagger-ui'), scroll: false });

            $('<i title="展开拓展功能" class="plug-in-all"><svg t="1615609432254" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1514" width="80" height="80"><path d="M119.049445 308.054588a496.562 165.864 30 1 0 860.070613 496.562 496.562 165.864 30 1 0-860.070613-496.562Z" fill="#80CFF4" p-id="1515"></path><path d="M546.378 1023.114c-50.304 0-96.306-54.79-129.534-154.274-31.754-95.07-49.244-221.226-49.244-355.218s17.49-260.144 49.246-355.22c33.228-99.48 79.23-154.264 129.534-154.264 50.302 0 96.304 54.786 129.534 154.264 31.758 95.078 49.246 221.23 49.246 355.22 0 133.992-17.492 260.148-49.246 355.218-33.23 99.488-79.234 154.274-129.536 154.274z m0-993.146c-73.864 0-152.946 194.344-152.946 483.656 0 289.308 79.082 483.656 152.946 483.656 73.866 0 152.95-194.348 152.95-483.656-0.002-289.312-79.086-483.656-152.95-483.656z" p-id="1516"></path><path d="M215.344 813.43c-55.07 0-93.176-15.582-110.196-45.058-25.152-43.562-0.708-110.796 68.83-189.312 66.458-75.042 166.968-153.264 283.01-220.262 154.886-89.428 315.982-144.978 420.42-144.978 55.072 0 93.18 15.584 110.2 45.062 25.152 43.566 0.706 110.796-68.832 189.314-66.458 75.04-166.964 153.258-283.008 220.262-154.892 89.42-315.99 144.972-420.424 144.972z m662.064-573.78c-100.166 0-256.312 54.226-407.506 141.518C219.352 525.824 90.584 691.484 127.52 755.458c15.338 26.566 56.226 32.144 87.826 32.144 100.166 0 256.31-54.226 407.506-141.52 113.696-65.64 211.924-142.002 276.586-215.016 60.692-68.524 84.67-126.578 65.798-159.27-15.34-26.57-56.228-32.146-87.828-32.146z" p-id="1517"></path><path d="M877.408 813.43c-104.438 0-265.534-55.55-420.418-144.972-116.042-67.004-216.55-145.222-283.008-220.264-69.538-78.516-93.984-145.75-68.83-189.314 17.02-29.48 55.126-45.062 110.2-45.062 104.432 0 265.528 55.552 420.416 144.978 116.042 66.998 216.548 145.222 283.008 220.26 69.542 78.52 93.986 145.75 68.832 189.308-17.02 29.484-55.128 45.066-110.2 45.066z m-662.058-573.78c-31.6 0-72.488 5.576-87.83 32.146-18.874 32.692 5.106 90.744 65.796 159.274 64.664 73.014 162.89 149.372 276.588 215.016 151.192 87.29 307.332 141.516 407.502 141.516 31.6 0 72.49-5.578 87.828-32.148 18.878-32.692-5.106-90.742-65.798-159.27-64.662-73.012-162.892-149.372-276.586-215.016-151.192-87.292-307.338-141.518-407.5-141.518z" p-id="1518"></path><path d="M259.54 51.118a14.782 14.782 0 0 1-9.29-3.3 14.652 14.652 0 0 1-5.384-9.928 14.672 14.672 0 0 1 3.22-10.828 14.692 14.692 0 0 1 11.466-5.464c3.37 0 6.67 1.17 9.292 3.294 6.318 5.128 7.286 14.442 2.162 20.762a14.698 14.698 0 0 1-11.466 5.464z m0.012-26.938c-3.692 0-7.14 1.644-9.462 4.508a12.096 12.096 0 0 0-2.654 8.932 12.08 12.08 0 0 0 4.44 8.188 12.192 12.192 0 0 0 7.662 2.722 12.14 12.14 0 0 0 9.462-4.506 12.194 12.194 0 0 0-1.784-17.126 12.066 12.066 0 0 0-7.664-2.718z" fill="#F18D00" p-id="1519"></path><path d="M78.344 41.29m-40.402 0a40.402 40.402 0 1 0 80.804 0 40.402 40.402 0 1 0-80.804 0Z" fill="#36B8F2" p-id="1520"></path><path d="M74.88 106.51a48.266 48.266 0 0 1-30.302-10.752 47.854 47.854 0 0 1-17.564-32.4 47.84 47.84 0 0 1 10.494-35.328 47.974 47.974 0 0 1 37.424-17.82 48.222 48.222 0 0 1 30.298 10.754c9.99 8.098 16.226 19.606 17.562 32.4s-2.39 25.338-10.49 35.326a47.98 47.98 0 0 1-37.422 17.82z m0.052-80.8c-9.904 0-19.16 4.404-25.386 12.086a32.446 32.446 0 0 0-7.116 23.956 32.434 32.434 0 0 0 11.912 21.972 32.274 32.274 0 0 0 20.542 7.292 32.54 32.54 0 0 0 25.386-12.086 32.422 32.422 0 0 0 7.11-23.956 32.436 32.436 0 0 0-11.908-21.972 32.27 32.27 0 0 0-20.54-7.292zM173.798 45.346a5.164 5.164 0 0 1-0.778-10.27l24.936-3.824a5.168 5.168 0 0 1 1.566 10.214l-24.938 3.822a5.492 5.492 0 0 1-0.786 0.058z" p-id="1521"></path><path d="M188.176 55.902a5.17 5.17 0 0 1-5.102-4.386l-3.822-24.938a5.168 5.168 0 0 1 10.214-1.566l3.822 24.938a5.17 5.17 0 0 1-5.112 5.952z" p-id="1522"></path><path d="M828.736 933.464H761.4a7.746 7.746 0 0 1-7.748-7.75 7.748 7.748 0 0 1 7.748-7.75h67.336a7.75 7.75 0 0 1 0 15.5z" fill="#E81F1F" p-id="1523"></path><path d="M795.066 967.128a7.746 7.746 0 0 1-7.748-7.746v-67.336a7.748 7.748 0 1 1 15.498 0v67.336a7.744 7.744 0 0 1-7.75 7.746z" fill="#E81F1F" p-id="1524"></path><path d="M883.948 925.716m-21.884 0a21.884 21.884 0 1 0 43.768 0 21.884 21.884 0 1 0-43.768 0Z" fill="#58B530" p-id="1525"></path><path d="M956.786 907.106a5.138 5.138 0 0 1-3.472-1.344l-21.78-19.782a5.164 5.164 0 1 1 6.944-7.648l21.78 19.782a5.168 5.168 0 0 1-3.472 8.992z" fill="#36B8F2" p-id="1526"></path><path d="M936.004 908.11a5.168 5.168 0 0 1-3.822-8.644l19.782-21.782a5.166 5.166 0 1 1 7.646 6.944l-19.782 21.786a5.148 5.148 0 0 1-3.824 1.696z" fill="#36B8F2" p-id="1527"></path></svg></i>')
                .appendTo($body)
                .on('click', (e) => {
                    var open = $(e.currentTarget).data('open');
                    $(e.currentTarget).data('open', open == 1 ? 0 : 1)
                    $(e.currentTarget).attr('title', open == 1 ? '展开拓展功能' : '收起拓展功能');
                    $(e.currentTarget.children[0]).animate({ 'height': (open == 1 ? 80 : 70) + 'px', 'width': (open == 1 ? 80 : 70) + 'px' });
                    $(e.currentTarget.parentElement).find('.plug-in').each((index, item) => {
                        if (open == 1)
                            $(item).animate({ 'opacity': 0, 'top': 0 + 'px' });
                        else
                            $(item).animate({ 'opacity': 1, 'top': (100 + index * 100) + 'px' });
                    });
                });

            //提醒
            var attention = (s = 0) => {
                if (s > 10)
                    return;
                $('.plug-in-all svg').animate({ 'height': (s % 2 != 0 ? 100 : 80) + 'px', 'width': (s % 2 != 0 ? 100 : 80) + 'px' }, 390, () => { attention(++s) });
            }
            attention(1);
        }

        if ($('#plug-in-' + name).length == 0)
            $('<i title="{title}" class="plug-in" id="plug-in-{name}">{svg}</i>'
                .format({ name: name, title: title, svg: svg }))
                .appendTo($body)
                .on('click', (e) => { fun(name, e); });
        else
            $('#plug-in-' + name)
                .empty()
                .html(svg)
                .attr('title', title)
                .off('click')
                .on('click', (e) => { fun(name, e); });
    }
}

var callback = function () {
    //返回顶部
    var $scrollToTop = $('<i title="返回顶部" class="scrollToTop" ><svg t="1615613258516" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="32909" width="80" height="80"><path d="M869.477693 469.30536L554.231089 22.857856c-19.341263-27.429428-65.152883-27.397459-84.494147 0L154.522307 469.30536a51.598014 51.598014 0 0 0-3.676439 53.644033c8.919359 17.199338 26.534295 27.908963 45.939496 27.908963h185.740064v296.704567a23.976773 23.976773 0 0 0 47.953545 0v-319.6903a23.497237 23.497237 0 0 0-2.014048-9.494802 23.880865 23.880865 0 0 0-22.250445-15.47301H196.753395c-0.575443 0-2.30177 0-3.324779-2.014049-1.054978-2.046018-0.063938-3.452655 0.255752-3.932191l315.214636-446.447504a3.772346 3.772346 0 0 1 6.138054 0l315.246605 446.447504c0.31969 0.479535 1.342699 1.886173 0.255752 3.900222-1.054978 2.014049-2.749337 2.014049-3.324779 2.014049h-208.565952l-0.159845 0.031969-0.159845-0.031969a23.976773 23.976773 0 0 0-23.976773 23.976772v263.520715a23.976773 23.976773 0 0 0 47.953545 0V550.826387h184.90887c19.373232 0 36.988168-10.709625 45.907527-27.908963a51.470138 51.470138 0 0 0-3.64447-53.612064zM618.360963 893.534389a23.976773 23.976773 0 0 0-23.976773 23.976772v81.552996a23.976773 23.976773 0 0 0 47.953545 0V917.511161a23.976773 23.976773 0 0 0-23.976772-23.976772z" fill="#438CFF" p-id="32910"></path><path d="M577.24879 288.232774m-31.96903 0a31.96903 31.96903 0 1 0 63.93806 0 31.96903 31.96903 0 1 0-63.93806 0Z" fill="#438CFF" p-id="32911"></path></svg></i>')
        .appendTo($("body"))
        .on('click', (e) => { $(document).scrollTop(0); });

    $(window).scroll(function (e) {
        h = $(window).height();
        t = $(document).scrollTop();
        if (t > h) {
            $scrollToTop.show();
        } else {
            $scrollToTop.hide();
        }
    });

    var loaded = () => {
        $('.operation-filter-input').attr('placeholder', '标签名称（区分大小写）')
            .on('change', (e) => {
                generateTags();
            });

        //生成左侧标签
        var generateTags = () => {
            var $tagBody = $('.tagsBody');
            if ($tagBody.length == 0)
                $tagBody = $('<div class="tagsBody"></div>')
                    .appendTo($('body'));

            $('.opblock-tag').each((index, item) => {
                var $tag = $('<span title="{1}">{0}</span>'.format(item.dataset['tag'], $(item).find('.renderedMarkdown p').text()))
                    .css({ 'top': (item.offsetTop + 5) + 'px' })
                    .appendTo($tagBody)
                    .on('click', (e) => {
                        $(document).scrollTop(item.offsetTop);
                    });

                $(window).scroll(function (e) {
                    var current = index * 75;
                    var valueA = item.offsetTop - $(document).scrollTop();
                    var valueB = valueA - current;
                    $tag.css({
                        'top': (valueB > 0 ? valueA : current + 10) + 'px'
                    });

                    if (valueB <= 0 && current >= window.outerHeight) {
                        $('.tagsBody span').each((indexB, itemB) => {
                            $(itemB).css({ 'top': (indexB - 1) * 75 - current + window.outerHeight + 'px' });
                        });
                    }
                });
            });
        };

        //监听HTML结构发生的变化
        var mutationObserver = window.MutationObserver
            || window.WebKitMutationObserver
            || window.MozMutationObserver;//浏览器兼容

        var observer = new mutationObserver((mutations) => {
            for (var index in mutations) {
                if (mutations[index].type == 'childList') {
                    $('.tagsBody').empty();
                    generateTags();
                    break;
                }
            }
        });
        observer.observe($(".opblock-tag-section")[0].parentElement.parentElement, { attributes: false, childList: true });

        generateTags();
    };

    var isLoaded = setInterval(() => {
        $('.information-container').length != 0 ? (window.clearInterval(isLoaded), loaded()) : 0;
    }, 100);
};

document.readyState === "complete" || (document.readyState !== "loading" && !document.documentElement.doScroll) ? callback() : document.addEventListener("DOMContentLoaded", callback);