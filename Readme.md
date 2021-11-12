# **NSystem.Data.Dabber**
本仓库基于[Dapper](https://github.com/DapperLib/Dapper)进行开发，因为之前为方便操作，写了一个简单的SQL自动生成的功能，使用中发现后续Dapper不支持较低版本的.NetFramework，所以将自己的代码和Dapper（v2.0.90）进行合并，从而衍生出此项目，后续会跟进Dapper的更新，使此项目的易用度得到提升，敬请期待！
## **类库介绍**
1.System.Data.Dabber<br/>
此命名空间是Dapper内容，有进行扩展的项，比如ALertable相关内容（AlertMsg相关、PagingResult相关）、DbConnectionPool相关，其余并未做过多修改，只是将不支持较低的.NetFramework，进行了兼容，增加了CompatAssist（兼容助手）类，修改了些注释，方便阅读。<br/>
2.System.Data.Cobber<br/>
此命名空间自动SQL系列，其中包括自助生成SQL类的AutoSqlBuilder系列，支持SQLite、MySQL、SqlServer、Access，通过DbCol标记，然后创建实体类的AutoSqlModel，生成一些常用的SQL语句；存储模型StoreModel，存放数据库类型及连接，通过此与Provider配合使用，支持切换数据库操作；CacheModel提供几种静态字典的缓存使用，提供简单的存取；DbMigration提供仿照EntityFramework的迁移形式进行迁移，使用方式很简单DbAutoMigration.Regist指定参数的迁移方法或IDbAutoMigration接口类；ADataAccess是一些简单操作，可使用此继承方式快速实现数据访问；ConnBuilder提供简单的创建、解析连接字符串等。<br/>
另外命名空间还包括加密系列，其中包括AppConfigJsonFile配置文件类；Newtonsoft.Json扩展；用户加密，如AES加密解密方法，其余使用UserPassword、UserFileable进行其余部分加密；UserExCell是针对Excel封装的行列模型对应；UserKeyGuid是将Guid或双Int64转换成64位/32位字符串的相互转换，以减少空间；<br/>
其他内容，如JavaEnum，仿照JavaEnum的实现形式实现的enum转换成类定义的实现；LazyBone仿照Lazy实现，目的是可以重新加载；MemberAccessor是使用反射、委托、表达式树三种形式实现的通过类型或泛型找到属性进行get和set的方式，注释提供性能参考；静态调用类，此类是一些泛型或者常用对象的扩展操作方法，不常用的或一些复杂的在Extter中有所体现等。<br/>
3.System.Data.Extter<br/>
常用扩展类，是一些本人开发中整理的扩展操作类及方法。常用的有类型的Caller类，未避免重复有些Caller类有两个命名空间，但是在同一个文件中，避免和Cobber中的已定义重复，不一一介绍了。<br/>
4.System.Data.Logger<br/>
此例中包含日志定义，当前实现有TextLogWriter，可以通过Console.SetOut进行设置并生效使用；SQLiteLogWriter尚在开发过程中，实际中未进行使用。<br/>
## **安装说明**
可以通过Nuget获取此包的发布

```
Install-Package NSystem.Data.Dabber
```
## **免责声明**
如侵犯原作者权益，望告知，发现问题立即整改。
