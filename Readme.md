# **NSystem.Data.Dabber**

本仓库最初基于[Dapper](https://github.com/DapperLib/Dapper)进行开发，因为之前为方便操作，写了一个简单的SQL自动生成的功能，使用中发现后续Dapper不支持较低版本的.NET Framework，所以将自己的代码和Dapper进行合并，加上其他项目中引用的基础类库内容，从而衍生出此项目。

## **类库介绍**

##### **0.System**

此中是针对应用中最频繁的内容进行的扩展，比如Alertable相关内容、Pagingable相关内容、Swappable相关内容、Testable相关内容。

##### **1.System.Data.Dabber**

此命名空间是基于[Dapper（v2.0.90）](https://github.com/DapperLib/Dapper)内容，有进行扩展的项，比如DbConnectionPool相关，其余并未做过多修改，只是将不支持较低的.NET Framework，进行了兼容，增加了CompatAssist（兼容助手）类，修改了些注释，方便阅读。后续会跟进Dapper的更新，使此项目的易用度得到提升，敬请期待！

##### **2.System.Data.Cobber**

此命名空间自动SQL系列，其中包括自助生成SQL类的AutoSqlBuilder系列，支持SQLite、MySQL、SqlServer、Access，通过DbCol标记，然后创建实体类的AutoSqlModel，生成一些常用的SQL语句；存储模型StoreModel，存放数据库类型及连接，通过此与Provider配合使用，支持切换数据库操作；CacheModel提供几种静态字典的缓存使用，提供简单的存取；DbMigration提供仿照EntityFramework的迁移形式进行迁移，使用方式很简单DbAutoMigration.Regist指定参数的迁移方法或IDbAutoMigration接口类；ADataAccess是一些简单操作，可使用此继承方式快速实现数据访问；ConnBuilder提供简单的创建、解析连接字符串等。<br/>
另外命名空间还包括加密系列，其中包括AppConfigJsonFile配置文件类；Newtonsoft.Json扩展；用户加密，如AES加密解密方法，其余使用UserPassword、UserFileable进行其余部分加密；UserExCell是针对Excel封装的行列模型对应；UserKeyGuid是将Guid或双Int64转换成64位/32位字符串的相互转换，以减少空间；<br/>
其他内容，如LazyBone仿照Lazy实现，目的是可以重新加载；MemberAccessor是使用反射、委托、表达式树三种形式实现的通过类型或泛型找到属性进行get和set的方式，注释提供性能参考；静态调用类，此类是一些泛型或者常用对象的扩展操作方法，不常用的或一些复杂的在Extter中有所体现等。

##### **3.System.Data.Extter**

常用扩展类，是一些本人开发中整理的扩展操作类及方法。常用的有类型的Caller类，未避免重复有些Caller类有两个命名空间，但是在同一个文件中，避免和Cobber中的已定义重复，不一一介绍了。

##### **4.System.Data.Logger**

此例中包含日志定义，当前实现有TextLogWriter，可以通过Console.SetOut进行设置并生效使用；SQLiteLogWriter尚在开发过程中，实际中未进行使用。

##### **5.System.Data.Chineser**

此命名空间是汉字转拼音及拼音字符串的一些内容，移植自[PinYinConverterCore](https://github.com/netcorepal/PinYinConverterCore.git)，因为需要兼容较低版本的.NET Framework，所以在项目中进行了集成。

##### **6.System.Data.Dibber**

此命名空间是后续进行Dabber、Expression、Emit等其他流行ORM融合的一个命名空间，是对数据挖掘内容的一个支持。

##### **7.System.Data.Dobber**

此命名空间是对于生成SQL语句进行ORM或者数据检索的一个应用命名空间，此例中的SqlScriptBuilder已经在使用中。

##### **8.System.Data.Dubber**

此命名空间是对于网络组件/通信组件进行封装的内容，其中包含[FluentFTP](https://github.com/robinrodricks/FluentFTP)，因为需要兼容较低版本的.NET Framework，所以在项目中进行了集成。另外此中的管道通信内容已在跨进程通信中使用。

##### **9.System.Data.Impeller**

此命名空间是对于动态链接库导入的一个应用内容封装，其中是常见的用到的一些系统内容。

##### **10.System.Data.Mabber**

此命名空间是对象映射进行封装的内容,其中包括[TinyMapper](https://github.com/TinyMapper/TinyMapper)，因为应用比较广泛所以在项目中进行了集成。

##### **11.System.Data.Hopper**

此命名空间是对网络调用进行封装的内容，因为后续HttpWebRequest等内容过期，需要无缝衔接到新式调用上，故创建此命名空间，目前HttpResStatusCode是约定使用。

##### **99.System.Data.Obsoleter**

此中无命名空间，只存放哪些标记Obsolete的内容。

## **安装说明**

可以通过Nuget获取此包的发布

```
Install-Package NSystem.Data.Dabber
```

## **免责声明**

如侵犯原作者权益，望告知，发现问题立即整改。
