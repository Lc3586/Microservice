
// 导入元数据支持
import "reflect-metadata";

// 存放所有可以被作为依赖项的类
const classPool: Array<Function> = [];

// 标记可被注入类
export function injectable(_constructor: Function) {
    // 通过反射机制，获取参数类型列表    
    let paramsTypes: Array<Function> = Reflect.getMetadata('design:paramtypes', _constructor);
    if (classPool.indexOf(_constructor) !== -1) {
        return;
    } else if (paramsTypes.length) {
        paramsTypes.forEach((v, i) => {
            if (v === _constructor) {
                throw new Error('不可以依赖自身');
            } else if (classPool.indexOf(v) === -1) {
                throw new Error(`依赖${i}[${(v as any).name}]不可被注入`);
            }
        });
    }
    classPool.push(_constructor);
}

// 创建实例
export function create<T>(_constructor: { new (...args: Array<any>): T }): T {
    // 通过反射机制，获取参数类型列表
    let paramsTypes: Array<Function> = Reflect.getMetadata('design:paramtypes', _constructor);
    // 实例化参数列表
    let paramInstances = paramsTypes.map((v, i) => {
        // 参数不可注入
        if (classPool.indexOf(v) === -1) {
            throw new Error(`参数${i}[${(v as any).name}]不可被注入`);
        // 参数有依赖项则递归实例化参数对象
        } else if (v.length) {
            return create(v as any);
        // 参数无依赖则直接创建对象
        } else {
            return new (v as any)();
        }
    });
    return new _constructor(...paramInstances);
}

@injectable
class B {
    public constructor(public a: E) {

    }
}
@injectable
class C {

}
@injectable
class D {

}
@injectable
class E {

}
@injectable
class A {
    public constructor(public b: B, public c: C, public d: D) {

    }
}

// 仅需要一行代码
let a = create(A);

//对比之前的代码 ， 如果依赖增加差距将更大
//var e = new E();
//var b = new B(e);
//var c = new C();
//var d = new D();
//var a = new A(b,c,d);