"use strict";(self["webpackChunklinker_web"]=self["webpackChunklinker_web"]||[]).push([[295],{4:function(e,t,a){a.d(t,{a1:function(){return h},e3:function(){return k},jH:function(){return C},zG:function(){return v}});a(4114),a(6573),a(8100),a(7936);var n=a(1219);let l=0,o=null,r="",s=1,i="";const d={},g={connected:!1,connecting:!1},u=()=>{const e=Date.now();for(let t in d){const a=d[t];e-a.time>a.timeout&&(a.reject("超时~"),delete d[t])}setTimeout(u,1e3)};u();const c={subs:{},add:function(e,t){"function"==typeof t&&(this.subs[e]||(this.subs[e]=[]),this.subs[e].push(t))},remove(e,t){let a=this.subs[e]||[];for(let n=a.length-1;n>=0;n--)a[n]==t&&a.splice(n,1)},push(e,t){let a=this.subs[e]||[];for(let n=a.length-1;n>=0;n--)a[n](t)}},p=()=>{g.connected=!0,g.connecting=!1,c.push(y,g.connected)},b=e=>{g.connected=!1,g.connecting=!1,c.push(y,g.connected),setTimeout((()=>{h()}),1e3)},f=e=>{if("string"!=typeof e.data)return void e.data.arrayBuffer().then((t=>{const a=new DataView(t).getInt8(),n=new FileReader;n.readAsText(e.data.slice(4,4+a),"utf8"),n.onload=()=>{let l=JSON.parse(n.result);l.Content={Name:l.Content,Img:e.data.slice(4+a,e.data.length),ArrayBuffer:t},m(l)}}));let t=JSON.parse(e.data);m(t)},m=e=>{let t=d[e.RequestId];t?(0==e.Code?t.resolve(e.Content):1==e.Code?t.reject(e.Content):255==e.Code?(t.reject(e.Content),t.errHandle||n.nk.error(`${t.path}:${e.Content}`)):c.push(e.Path,e.Content),delete d[e.RequestId]):c.push(e.Path,e.Content)},h=(e=r,t=i)=>{if(i=t,r=e,g.connecting||g.connected)return;null!=o&&o.close(),g.connecting=!0;const a=t||"snltty";o=new WebSocket(r,[a]),o.iddd=++s,o.onopen=p,o.onclose=b,o.onmessage=f},k=()=>{o&&o.close()},v=(e,t={},a=!1,n=15e3)=>new Promise(((r,s)=>{let i=++l;try{d[i]={resolve:r,reject:s,errHandle:a,path:e,time:Date.now(),timeout:n};let l=JSON.stringify({Path:e,RequestId:i,Content:"string"==typeof t?t:JSON.stringify(t)});g.connected&&1==o.readyState?o.send(l):s("网络错误~")}catch(u){console.log(u),s("网络错误~"),delete d[i]}})),y=Symbol(),C=e=>{c.add(y,e)}},3830:function(e,t,a){a.d(t,{B:function(){return i},v:function(){return s}});var n=a(4),l=a(144),o=a(6768);const r=Symbol(),s=()=>{const e=(0,l.KR)({api:{connected:!1},height:0,config:{Common:{},Client:{Servers:[]},Server:{},Running:{Relay:{Servers:[]},Tuntap:{IP:"",PrefixLength:24},Client:{Servers:[]},AutoSyncs:{}},configed:!1},signin:{Connected:!1,Connecting:!1,Version:"v1.0.0.0"},bufferSize:["1KB","2KB","4KB","8KB","16KB","32KB","64KB","128KB","256KB","512KB","1024KB"],updater:{},self:{}});return(0,n.jH)((t=>{e.value.api.connected=t})),(0,o.Gt)(r,e),e},i=()=>(0,o.WQ)(r)},4295:function(e,t,a){a.r(t),a.d(t,{default:function(){return B}});var n=a(6768),l=a(4232);const o=e=>((0,n.Qi)("data-v-698722d0"),e=e(),(0,n.jt)(),e),r={class:"logger-setting-wrap flex flex-column h-100",ref:"wrap"},s={class:"inner"},i={class:"head flex"},d=o((()=>(0,n.Lk)("span",{class:"flex-1"},null,-1))),g={class:"body flex-1 relative"},u={key:0},c={class:"pages t-c"},p={class:"page-wrap"};function b(e,t,a,o,b,f){const m=(0,n.g2)("el-option"),h=(0,n.g2)("el-select"),k=(0,n.g2)("el-button"),v=(0,n.g2)("el-table-column"),y=(0,n.g2)("el-table"),C=(0,n.g2)("el-empty"),w=(0,n.g2)("el-pagination"),F=(0,n.g2)("el-tab-pane"),S=(0,n.g2)("Setting"),z=(0,n.g2)("el-tabs");return(0,n.uX)(),(0,n.CE)("div",r,[(0,n.bF)(z,{type:"border-card"},{default:(0,n.k6)((()=>[(0,n.bF)(F,{label:"主页"},{default:(0,n.k6)((()=>[(0,n.Lk)("div",s,[(0,n.Lk)("div",i,[(0,n.Lk)("div",null,[(0,n.bF)(h,{modelValue:o.state.type,"onUpdate:modelValue":t[0]||(t[0]=e=>o.state.type=e),onChange:o.loadData,size:"small",class:"m-r-1",style:{width:"6rem"}},{default:(0,n.k6)((()=>[(0,n.bF)(m,{value:-1,label:"all"}),(0,n.bF)(m,{value:0,label:"debug"}),(0,n.bF)(m,{value:1,label:"info"}),(0,n.bF)(m,{value:2,label:"warning"}),(0,n.bF)(m,{value:3,label:"error"}),(0,n.bF)(m,{value:4,label:"fatal"})])),_:1},8,["modelValue","onChange"])]),(0,n.bF)(k,{type:"warning",size:"small",loading:o.state.loading,onClick:o.clearData},{default:(0,n.k6)((()=>[(0,n.eW)("清空")])),_:1},8,["loading","onClick"]),(0,n.bF)(k,{size:"small",loading:o.state.loading,onClick:o.loadData},{default:(0,n.k6)((()=>[(0,n.eW)("刷新列表")])),_:1},8,["loading","onClick"]),d]),(0,n.Lk)("div",g,[o.state.page.List.length>0?((0,n.uX)(),(0,n.CE)("div",u,[(0,n.bF)(y,{border:"",data:o.state.page.List,size:"small",height:`${o.state.height}px`,onRowClick:o.handleRowClick,"row-class-name":o.tableRowClassName},{default:(0,n.k6)((()=>[(0,n.bF)(v,{type:"index",width:"50"}),(0,n.bF)(v,{prop:"Type",label:"类别",width:"80"},{default:(0,n.k6)((e=>[(0,n.Lk)("span",null,(0,l.v_)(o.state.types[e.row.Type]),1)])),_:1}),(0,n.bF)(v,{prop:"Time",label:"时间",width:"160"}),(0,n.bF)(v,{prop:"content",label:"内容"})])),_:1},8,["data","height","onRowClick","row-class-name"])])):((0,n.uX)(),(0,n.Wv)(C,{key:1}))]),(0,n.Lk)("div",c,[(0,n.Lk)("div",p,[(0,n.bF)(w,{small:"",total:o.state.page.Count,currentPage:o.state.page.Page,"onUpdate:currentPage":t[1]||(t[1]=e=>o.state.page.Page=e),"page-size":o.state.page.Size,onCurrentChange:o.handlePageChange,background:"",layout:"total,prev, pager, next"},null,8,["total","currentPage","page-size","onCurrentChange"])])])])])),_:1}),(0,n.bF)(F,{label:"配置"},{default:(0,n.k6)((()=>[(0,n.bF)(S)])),_:1})])),_:1})],512)}var f=a(144),m=a(4);const h=e=>(0,m.zG)("loggerclient/get",e),k=()=>(0,m.zG)("loggerclient/clear"),v=()=>(0,m.zG)("loggerclient/getconfig"),y=e=>(0,m.zG)("loggerclient/setconfig",e),C={class:"t-c w-100"};function w(e,t,a,l,o,r){const s=(0,n.g2)("el-input"),i=(0,n.g2)("el-form-item"),d=(0,n.g2)("el-col"),g=(0,n.g2)("el-option"),u=(0,n.g2)("el-select"),c=(0,n.g2)("el-row"),p=(0,n.g2)("el-button"),b=(0,n.g2)("el-form");return(0,n.uX)(),(0,n.Wv)(b,{"label-width":"8rem",ref:"formDom",model:l.state.form,rules:l.state.rules},{default:(0,n.k6)((()=>[(0,n.bF)(i,{label:"","label-width":"0"},{default:(0,n.k6)((()=>[(0,n.bF)(c,null,{default:(0,n.k6)((()=>[(0,n.bF)(d,{xs:24,sm:8,md:8,lg:8,xl:8},{default:(0,n.k6)((()=>[(0,n.bF)(i,{label:"显示数量",prop:"Size"},{default:(0,n.k6)((()=>[(0,n.bF)(s,{size:"default",modelValue:l.state.form.Size,"onUpdate:modelValue":t[0]||(t[0]=e=>l.state.form.Size=e)},null,8,["modelValue"])])),_:1})])),_:1}),(0,n.bF)(d,{xs:24,sm:8,md:8,lg:8,xl:8},{default:(0,n.k6)((()=>[(0,n.bF)(i,{label:"日志等级",prop:"LoggerType"},{default:(0,n.k6)((()=>[(0,n.bF)(u,{modelValue:l.state.form.LoggerType,"onUpdate:modelValue":t[1]||(t[1]=e=>l.state.form.LoggerType=e)},{default:(0,n.k6)((()=>[(0,n.bF)(g,{value:0,label:"debug"}),(0,n.bF)(g,{value:1,label:"info"}),(0,n.bF)(g,{value:2,label:"warning"}),(0,n.bF)(g,{value:3,label:"error"}),(0,n.bF)(g,{value:4,label:"fatal"})])),_:1},8,["modelValue"])])),_:1})])),_:1})])),_:1})])),_:1}),(0,n.bF)(i,{"label-width":"0"},{default:(0,n.k6)((()=>[(0,n.Lk)("div",C,[(0,n.bF)(p,{type:"primary",loading:l.state.loading,onClick:l.submit},{default:(0,n.k6)((()=>[(0,n.eW)("确 定")])),_:1},8,["loading","onClick"])])])),_:1})])),_:1},8,["model","rules"])}var F=a(1219),S={setup(){const e=(0,f.KR)(null),t=(0,f.Kh)({loading:!1,configInfo:{},form:{Size:0,LoggerType:-1},rules:{Size:[{required:!0,message:"必填",trigger:"blur"},{type:"number",min:1,max:1e4,message:"数字 1-10000",trigger:"blur",transform(e){return Number(e)}}]}}),a=()=>{v().then((e=>{t.configInfo=e,t.form.Size=e.Size,t.form.LoggerType=e.LoggerType})).catch((e=>{}))},l=()=>{let e=JSON.parse(JSON.stringify(t.configInfo));return e.Size=+t.form.Size,e.LoggerType=+t.form.LoggerType,e},o=()=>new Promise(((a,n)=>{e.value.validate((e=>{if(0==e)return n(),!1;t.loading=!0;const o=l();y(o).then((e=>{t.loading=!1,a(),e?F.nk.success("操作成功!"):F.nk.error("操作失败!")})).catch((()=>{t.loading=!1,a()}))}))}));return(0,n.sV)((()=>{(0,m.jH)((e=>{e&&a()})),a()})),{state:t,formDom:e,submit:o}}},z=a(1241);const _=(0,z.A)(S,[["render",w],["__scopeId","data-v-3d9ad497"]]);var L=_,x=a(2933),P=a(3830),T={components:{Setting:L},setup(){const e=(0,P.B)(),t=(0,f.KR)(null),a=(0,f.Kh)({loading:!0,type:-1,page:{Page:1,Size:20,Count:0,List:[]},types:["debug","info","warning","error","fatal"],height:(0,f.EW)((()=>e.value.height-200))}),l=()=>{a.loading=!0,h({Page:a.page.Page,Size:a.page.Size,Type:a.type}).then((e=>{a.loading=!1,e.List.map((e=>{e.content=e.Content.substring(0,50)})),a.page=e})).catch((e=>{console.log(e),a.loading=!1}))},o=e=>{e&&(a.page.Page=e,l())},r=()=>{a.loading=!0,k().then((()=>{a.loading=!1,l()})).catch((()=>{a.loading=!1}))},s=({row:e,rowIndex:t})=>`type-${e.Type}`,i=(e,t,a)=>{let n="padding:1rem;border:1px solid #ddd; resize:none;width:39rem;box-sizing: border-box;white-space: nowrap; height:30rem;";x.s.alert(`<textarea class="scrollbar-4" style="${n}">${e.Content}</textarea>`,"内容",{dangerouslyUseHTMLString:!0}).catch((()=>{}))};return(0,n.sV)((()=>{(0,m.jH)((e=>{e&&l()})),l()})),{wrap:t,state:a,loadData:l,clearData:r,tableRowClassName:s,handleRowClick:i,handlePageChange:o}}};const K=(0,z.A)(T,[["render",b],["__scopeId","data-v-698722d0"]]);var B=K}}]);