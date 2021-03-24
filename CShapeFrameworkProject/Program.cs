using System;
using System.Reflection;
using System.Reflection.Emit;


namespace CShapeFrameworkProject
{
    public interface IPerson
    {
        void SayHello();
    }

    class Person2
    { 
        public void SayHello()
        {
            Console.WriteLine("Person 2 hello world");
        }
    }


    class Test
    {
        int idid;
        string namename;
        Test(int id, string name)
        {
            idid = id;
            namename = name;
        }

        Test() : this(1,"ssw")
        {
        }

        void Dttp<T>(T p)
        {
            string a = p.ToString();
        }

        int Tttp(int p)
        {
            int a = p + this.idid;
            return a;
        }

        void TestSet(int p)
        {
            idid = p;
        }

        int IntAdd(int p)
        {
            return p + idid;
        }

        string TestPrefix(string str, int printCount)
        {
            try
            {
                if(printCount < 0)
                {
                    throw new Exception("count is zero");
                }

                string prefix = "sss";
                string result = str + prefix;
                for (int i = 0;i<printCount;++i)
                {
                    Console.WriteLine(result);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error happened : " + e.Message);
            }
            return null;
        }

        static string temp;
        static int inx;
        static void TestString(int index, string param)
        {
            int aaa = 13;
            inx = index;
            temp = param;
            aaa = index + aaa;
        }


        static void Main(string[] args)
        {
            IPerson p = (IPerson)CreateType();
            p.SayHello();

            Type t = p.GetType();

            MethodInfo addiMethod = t.GetMethod("AdditionalNumber");

            PropertyInfo propertyInfo = t.GetProperty("Number");
            Console.WriteLine("默认值" + propertyInfo.GetValue(p, null));
            propertyInfo.SetValue(p, 5, null);
            Console.WriteLine("设置新值" + propertyInfo.GetValue(p, null));

            Console.WriteLine("调用方法" + addiMethod.Invoke(p, new object[]{ 5}));

            FieldInfo nameField = t.GetField("Name");
            Console.WriteLine("字段原值 ：" + nameField.GetValue(p));
            string name = "这是测试字符串";
            nameField.SetValue(p, name);
            Console.WriteLine(nameField.GetValue(p));

            Console.WriteLine();
            Console.WriteLine("测试字符串拼接");
            MethodInfo prefixMethod = t.GetMethod("AddPrefix");
            string result = prefixMethod.Invoke(p, new object[] { "baidu.com", 5 }).ToString();

            Console.WriteLine("AddPrefix result : " + result);

            uint nu = 2334222422;
            uint num = 23_3422_2422;
            Console.WriteLine(num);

            TestString(1, "ss");

            Console.ReadLine();
        }

        public static object CreateType()
        {
            AssemblyName assemblyName = new AssemblyName("assemblyName");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("PersonModule", "Person.dll", true);
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Person", TypeAttributes.Public);

            // 添加私有字段
            FieldBuilder fbNumber = typeBuilder.DefineField(
                "m_number",
                typeof(int),
                FieldAttributes.Private);

            // 添加公开字段
            FieldBuilder fbString = typeBuilder.DefineField(
                "Name",
                typeof(string),
                FieldAttributes.Public
                );

            // 定义带参数构造
            ConstructorBuilder ctor1 = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(int), typeof(string)}
                );
            ILGenerator ctor1IL = ctor1.GetILGenerator();
            // 对于构造方法而言，第一个参数是新实例的引用，再使用参数之前需要先将它push到栈中
            // 应该说，对于所有非静态实例，第0个参数都是对象本身，即this。
            // 需要注意的是，调用对象中的字段或者属性时，需要先将this放入计算堆栈，即ldarg_0;
            ctor1IL.Emit(OpCodes.Ldarg_0);
            // 调用base的构造
            ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            // 和前面一样
            ctor1IL.Emit(OpCodes.Ldarg_0);
            // 使用真正的参数
            ctor1IL.Emit(OpCodes.Ldarg_1);
            ctor1IL.Emit(OpCodes.Stfld, fbNumber);
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Ldarg_2);
            ctor1IL.Emit(OpCodes.Stfld, fbString);
            ctor1IL.Emit(OpCodes.Ret);

            // 定义无参数默认构造
            ConstructorBuilder ctor0 = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes
                );
            ILGenerator ctor0IL = ctor0.GetILGenerator();
            ctor0IL.Emit(OpCodes.Ldarg_0);
            ctor0IL.Emit(OpCodes.Ldc_I4_S, 65);
            ctor0IL.Emit(OpCodes.Ldstr, "默认名");
            // 调用有参构造
            ctor0IL.Emit(OpCodes.Call, ctor1);
            ctor0IL.Emit(OpCodes.Ret);

            // 定义属性
            PropertyBuilder pbNumber = typeBuilder.DefineProperty(
                "Number",
                PropertyAttributes.HasDefault,
                typeof(int),
                null);

            // 定义Get方法
            MethodBuilder mbNumberGetAccessor = typeBuilder.DefineMethod(
                "GetNumber",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(int),
                Type.EmptyTypes);

            ILGenerator numberGetIL = mbNumberGetAccessor.GetILGenerator();
            numberGetIL.Emit(OpCodes.Ldarg_0);
            numberGetIL.Emit(OpCodes.Ldfld, fbNumber);
            numberGetIL.Emit(OpCodes.Ret);

            // 定义Set方法
            MethodBuilder mbNumberSetAccessor = typeBuilder.DefineMethod(
                "SetNumber",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new Type[] { typeof(int) });
            ILGenerator numberSetIL = mbNumberSetAccessor.GetILGenerator();
            numberSetIL.Emit(OpCodes.Ldarg_0);
            numberSetIL.Emit(OpCodes.Ldarg_1);
            numberSetIL.Emit(OpCodes.Stfld, fbNumber);
            numberSetIL.Emit(OpCodes.Ret);

            // 将方法绑定到属性上
            pbNumber.SetGetMethod(mbNumberGetAccessor);
            pbNumber.SetSetMethod(mbNumberSetAccessor);

            // 定义一个增加number方法
            MethodBuilder mbAdditionalAccessor = typeBuilder.DefineMethod(
                "AdditionalNumber",
                MethodAttributes.Public,
                typeof(int),
                new Type[] { typeof(int) }
                );

            ILGenerator numberAdditionalIL = mbAdditionalAccessor.GetILGenerator();
            numberAdditionalIL.DeclareLocal(typeof(int)).SetLocalSymInfo("result");
            
            numberAdditionalIL.Emit(OpCodes.Ldarg_1);
            numberAdditionalIL.Emit(OpCodes.Ldarg_0);
            numberAdditionalIL.Emit(OpCodes.Ldfld, fbNumber);
            numberAdditionalIL.Emit(OpCodes.Add);
            numberAdditionalIL.Emit(OpCodes.Stloc_0);
            numberAdditionalIL.Emit(OpCodes.Ldloc_0);
            numberAdditionalIL.Emit(OpCodes.Ret);

            // 增加一个字符串拼接方法
            MethodBuilder mbAddPrefixAccessor = typeBuilder.DefineMethod(
                "AddPrefix",
                MethodAttributes.Public,
                typeof(string),
                new Type[] { typeof(string), typeof(int)}
                );
            ILGenerator prefixIL = mbAddPrefixAccessor.GetILGenerator();
            Label trylabel = prefixIL.DefineLabel();
            Label loopLabel = prefixIL.DefineLabel();
            Label beginLable = prefixIL.DefineLabel();
            Label endLable = prefixIL.DefineLabel();

            prefixIL.DeclareLocal(typeof(string)).SetLocalSymInfo("prefix");
            prefixIL.DeclareLocal(typeof(string)).SetLocalSymInfo("result");
            prefixIL.DeclareLocal(typeof(int)).SetLocalSymInfo("i");
            prefixIL.DeclareLocal(typeof(string)).SetLocalSymInfo("e");

            // try
            Label tryLable = prefixIL.BeginExceptionBlock();
            prefixIL.Emit(OpCodes.Ldarg_2);
            prefixIL.Emit(OpCodes.Ldc_I4_0);
            prefixIL.Emit(OpCodes.Bgt, trylabel);

            // throw exception
            prefixIL.Emit(OpCodes.Ldstr, "count is zero");
            prefixIL.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[] { typeof(string) }));
            prefixIL.Emit(OpCodes.Throw);

            prefixIL.MarkLabel(trylabel);
            prefixIL.Emit(OpCodes.Ldstr, "https://");
            prefixIL.Emit(OpCodes.Stloc_0);
            prefixIL.Emit(OpCodes.Ldloc_0);
            prefixIL.Emit(OpCodes.Ldarg_1);
            prefixIL.Emit(OpCodes.Call, typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }));
            prefixIL.Emit(OpCodes.Stloc_1);

            prefixIL.Emit(OpCodes.Ldc_I4_0);
            prefixIL.Emit(OpCodes.Stloc_2);
            prefixIL.Emit(OpCodes.Br, beginLable);

            prefixIL.MarkLabel(loopLabel);
            prefixIL.Emit(OpCodes.Ldloc_1);
            prefixIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

            prefixIL.Emit(OpCodes.Ldloc_2);
            prefixIL.Emit(OpCodes.Ldc_I4_1);
            prefixIL.Emit(OpCodes.Add);
            prefixIL.Emit(OpCodes.Stloc_2);

            prefixIL.MarkLabel(beginLable);
            prefixIL.Emit(OpCodes.Ldloc_2);
            prefixIL.Emit(OpCodes.Ldarg_2);
            prefixIL.Emit(OpCodes.Blt, loopLabel);
            prefixIL.Emit(OpCodes.Leave, endLable);

            // catch
            prefixIL.BeginCatchBlock(typeof(Exception));
            prefixIL.Emit(OpCodes.Callvirt, typeof(Exception).GetMethod("get_Message"));
            prefixIL.Emit(OpCodes.Stloc_3);
            prefixIL.Emit(OpCodes.Ldstr, " Error happened ");
            prefixIL.Emit(OpCodes.Ldloc_3);
            prefixIL.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }));
            prefixIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

            prefixIL.EndExceptionBlock();

            prefixIL.Emit(OpCodes.Ldnull);
            prefixIL.Emit(OpCodes.Stloc_1);

            prefixIL.MarkLabel(endLable);
            prefixIL.Emit(OpCodes.Ldloc_1);

            prefixIL.Emit(OpCodes.Ret);


            // 添加接口
            typeBuilder.AddInterfaceImplementation(typeof(IPerson));

            // 实现方法
            MethodBuilder mbim = typeBuilder.DefineMethod("SayHello",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot
                | MethodAttributes.Virtual | MethodAttributes.Final,
                null,
                Type.EmptyTypes);
            ILGenerator il = mbim.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "The Sayhello implememtation of IPerson");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(mbim, typeof(IPerson).GetMethod("SayHello"));

            Type personType = typeBuilder.CreateType();
            assemblyBuilder.Save("Person.dll");

            
            return Activator.CreateInstance(personType);
        }
    }
}
