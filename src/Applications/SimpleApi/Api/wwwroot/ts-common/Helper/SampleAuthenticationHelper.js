var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var SampleAuthenticationHelper = (function () {
    function SampleAuthenticationHelper() {
    }
    SampleAuthenticationHelper.prototype.init = function (renderData, axios, headers) {
        if (axios === void 0) { axios = null; }
        if (headers === void 0) { headers = null; }
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) {
                    _this.RenderData = renderData;
                    _this.Headers = headers;
                    var setup = function () { return __awaiter(_this, void 0, void 0, function () {
                        return __generator(this, function (_a) {
                            this.AxiosInstance = this.createAxiosInstance();
                            resolve(this.AxiosInstance);
                            return [2];
                        });
                    }); };
                    if (axios != null) {
                        _this.Axios = axios;
                        setup();
                    }
                    else {
                        ImportHelper.ImportFile([
                            {
                                Tag: "script",
                                Attributes: {
                                    type: 'text/javascript',
                                    src: ApiUri.BaseUrl + '/utils/axios.min.js'
                                }
                            }
                        ], function () {
                            _this.Axios = new window.axios;
                            setup();
                        });
                    }
                });
                return [2, promise];
            });
        });
    };
    SampleAuthenticationHelper.prototype.createAxiosInstance = function () {
        var axiosInstance = this.Axios.create({});
        if (this.Headers != null) {
            for (var key in this.Headers) {
                axiosInstance.defaults.headers.common[key] = this.Headers[key];
            }
        }
        return axiosInstance;
    };
    SampleAuthenticationHelper.prototype.addLoginFilter = function (axiosInstance, validationFailed) {
        axiosInstance.interceptors.response.use(function (response) {
            return response;
        }, function (error) {
            if (error && error.response && error.response.status == 401) {
                validationFailed && validationFailed();
                return Promise.reject(new Error(error.response.data));
            }
            else if (error && error.response && error.response.status == 403) {
                error.message = "无权限!";
                return Promise.reject(new Error(error.response.data));
            }
            else
                return Promise.reject(error);
        });
    };
    SampleAuthenticationHelper.prototype.authorized = function () {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var _this = this;
                    return __generator(this, function (_a) {
                        this.RenderData.Loading = true;
                        this.AxiosInstance.post(ApiUri.SAAuthorized).then(function (response) {
                            _this.RenderData.Loading = false;
                            if (response && response.data && response.data.Success)
                                resolve(response.data.Data);
                            else
                                reject(new Error(response.data.Message));
                        }).catch(function (error) {
                            console.error(error);
                            _this.RenderData.Loading = false;
                            reject(error);
                        });
                        return [2];
                    });
                }); });
                return [2, promise];
            });
        });
    };
    SampleAuthenticationHelper.prototype.login = function () {
        return __awaiter(this, void 0, void 0, function () {
            var promise;
            var _this = this;
            return __generator(this, function (_a) {
                promise = new Promise(function (resolve, reject) { return __awaiter(_this, void 0, void 0, function () {
                    var data;
                    var _this = this;
                    return __generator(this, function (_a) {
                        console.debug('login!');
                        console.debug(this.RenderData);
                        this.RenderData.Loading = true;
                        data = {
                            Account: this.RenderData.Username,
                            Password: this.RenderData.Password
                        };
                        this.AxiosInstance.post(ApiUri.SALogin, data).then(function (response) {
                            if (response.data.Success) {
                                _this.RenderData.Username = '';
                                _this.RenderData.Password = '';
                                resolve(response.data.Data);
                            }
                            else
                                reject(new Error(response.data.Message));
                        }).catch(function (error) {
                            console.error(error);
                            _this.RenderData.Loading = false;
                            reject(new Error('登录时发生异常'));
                        });
                        return [2];
                    });
                }); });
                return [2, promise];
            });
        });
    };
    SampleAuthenticationHelper.prototype.logout = function (returnUrl) {
        window.location.href = ApiUri.SALogout(returnUrl);
    };
    return SampleAuthenticationHelper;
}());
//# sourceMappingURL=SampleAuthenticationHelper.js.map