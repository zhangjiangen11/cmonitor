"use strict";(self["webpackChunklinker_web"]=self["webpackChunklinker_web"]||[]).push([[889],{5241:function(e,t,n){n.d(t,{AE:function(){return p},Jd:function(){return d},Nj:function(){return h},PR:function(){return u},Yh:function(){return o},ac:function(){return r},en:function(){return c},kl:function(){return s},vB:function(){return a},zi:function(){return l}});var i=n(4);const s=(e="0")=>(0,i.zG)("tuntapclient/connections",e),a=e=>(0,i.zG)("tuntapclient/removeconnection",e),l=(e="0")=>(0,i.zG)("tuntapclient/get",e),c=e=>(0,i.zG)("tuntapclient/run",e),u=e=>(0,i.zG)("tuntapclient/stop",e),r=e=>(0,i.zG)("tuntapclient/update",e),o=()=>(0,i.zG)("tuntapclient/refresh"),d=()=>(0,i.zG)("tuntapclient/SubscribePing"),h=()=>(0,i.zG)("tuntapclient/GetNetwork"),p=e=>(0,i.zG)("tuntapclient/AddNetwork",e)},7985:function(e,t,n){n.d(t,{r:function(){return u}});n(4114);var i=n(9299),s=n(3830),a=n(6768),l=n(144);const c=[],u=()=>{const e=(0,s.B)(),t=(0,a.EW)((()=>e.value.config.Client.Id)),n=(0,l.Kh)({timer:0,page:{Request:{Page:1,Size:+(localStorage.getItem("ps")||"10"),Name:"",Ids:[],Prop:"",Asc:!0},Count:0,List:[]},showDeviceEdit:!1,showAccessEdit:!1,deviceInfo:null}),u=()=>{(0,i.nD)(n.page.Request).then((i=>{n.page.Request=i.Request,n.page.Count=i.Count;for(let n in i.List)Object.assign(i.List[n],{showDel:t.value!=i.List[n].MachineId&&0==i.List[n].Connected,showAccess:t.value!=i.List[n].MachineId&&i.List[n].Connected,showReboot:i.List[n].Connected,isSelf:t.value==i.List[n].MachineId,showip:!1}),i.List[n].isSelf&&(e.value.self=i.List[n]);n.page.List=i.List;for(let e=0;e<n.page.List.length;e++)c.push(n.page.List[e])})).catch((e=>{}))},r=()=>{(0,i.nD)(n.page.Request).then((i=>{for(let s in i.List){const a=n.page.List.filter((e=>e.MachineId==i.List[s].MachineId))[0];a&&(Object.assign(a,{Connected:i.List[s].Connected,Version:i.List[s].Version,LastSignIn:i.List[s].LastSignIn,Args:i.List[s].Args,showDel:t.value!=i.List[s].MachineId&&0==i.List[s].Connected,showAccess:t.value!=i.List[s].MachineId&&i.List[s].Connected,showReboot:i.List[s].Connected,isSelf:t.value==i.List[s].MachineId}),a.isSelf&&(e.value.self=a))}n.timer=setTimeout(r,5e3)})).catch((e=>{n.timer=setTimeout(r,5e3)}))},o=()=>{try{if(0==c.length)return void setTimeout(o,1e3);const e=c.shift();fetch(`http://ip-api.com/json/${e.IP.split(":")[0]}`).then((async t=>{try{const n=await t.json();e.countryFlag=`https://unpkg.com/flag-icons@7.2.3/flags/4x3/${n.countryCode.toLowerCase()}.svg`}catch(n){}setTimeout(o,1e3)})).catch((()=>{setTimeout(o,1e3)}))}catch(e){setTimeout(o,1e3)}};o();const d=e=>{n.deviceInfo=e,n.showDeviceEdit=!0},h=e=>{n.deviceInfo=e,n.showAccessEdit=!0},p=e=>{e&&(n.page.Request.Page=e),u()},m=e=>{e&&(n.page.Request.Size=e,localStorage.setItem("ps",e)),u()},f=e=>{(0,i.Se)(e).then((()=>{u()}))},g=()=>{clearTimeout(n.timer),n.timer=0},v=e=>(0,i.VN)(e);return{devices:n,machineId:t,_getSignList:u,_getSignList1:r,handleDeviceEdit:d,handleAccessEdit:h,handlePageChange:p,handlePageSizeChange:m,handleDel:f,clearDevicesTimeout:g,setSort:v}}},8104:function(e,t,n){n.d(t,{O:function(){return c},W:function(){return u}});var i=n(144),s=n(6768),a=n(5241);const l=Symbol(),c=()=>{const e=(0,i.KR)({timer:0,showEdit:!1,current:null,list:{},hashcode:0,showLease:!1});(0,s.Gt)(l,e);const t={linux:["debian","ubuntu","alpine","rocky","centos"],windows:["windows"],android:["android"],ios:["ios"]},n=()=>{clearTimeout(e.value.timer),(0,a.zi)(e.value.hashcode.toString()).then((i=>{if(e.value.hashcode=i.HashCode,i.List){for(let e in i.List){let n="system";const s=i.List[e].SystemInfo.toLowerCase();for(let e in t)if(s.indexOf(e)>=0){const i=t[e];if(1==i.length)n=i[0];else for(let e=0;e<i.length;e++)if(s.indexOf(i[e])>=0){n=i[e];break}break}Object.assign(i.List[e],{running:2==i.List[e].Status,loading:1==i.List[e].Status,system:n,systemDocker:s.indexOf("docker")>=0})}e.value.list=i.List}e.value.timer=setTimeout(n,1100),(0,a.Jd)()})).catch((t=>{e.value.timer=setTimeout(n,1100)}))},c=t=>{e.value.current=t,e.value.showEdit=!0},u=()=>{(0,a.Yh)()},r=()=>{clearTimeout(e.value.timer),e.value.timer=0},o=t=>Object.values(e.value.list).filter((e=>e.IP.indexOf(t)>=0||e.Lans.filter((e=>e.IP.indexOf(t)>=0)).length>0)).map((e=>e.MachineId)),d=t=>{const n=Object.values(e.value.list).filter((e=>e.IP)).sort(((e,t)=>{const n=e.IP.split(".").map((e=>Number(e))),i=t.IP.split(".").map((e=>Number(e)));for(let s=0;s<n.length;s++)if(n[s]!=i[s])return n[s]-i[s];return 0}));return n.map((e=>e.MachineId))};return{tuntap:e,_getTuntapInfo:n,handleTuntapEdit:c,handleTuntapRefresh:u,clearTuntapTimeout:r,getTuntapMachines:o,sortTuntapIP:d}},u=()=>(0,s.WQ)(l)},5660:function(e,t,n){n.d(t,{A:function(){return I}});var i=n(6768),s=n(4232);const a=["title"],l=["src"],c=["src"],u={key:1,class:"system",src:"/docker.svg"},r={class:"gateway"},o={key:1,class:"self gateway"};function d(e,t,n,d,h,p){const m=(0,i.g2)("StarFilled"),f=(0,i.g2)("el-icon");return(0,i.uX)(),(0,i.CE)("div",null,[d.tuntap.list[d.item.MachineId]&&d.tuntap.list[d.item.MachineId].system?((0,i.uX)(),(0,i.CE)("span",{key:0,title:d.tuntap.list[d.item.MachineId].SystemInfo},[d.item.countryFlag?((0,i.uX)(),(0,i.CE)("img",{key:0,class:"system",src:d.item.countryFlag},null,8,l)):(0,i.Q3)("",!0),(0,i.Lk)("img",{class:"system",src:`/${d.tuntap.list[d.item.MachineId].system}.svg`},null,8,c),d.tuntap.list[d.item.MachineId].systemDocker?((0,i.uX)(),(0,i.CE)("img",u)):(0,i.Q3)("",!0)],8,a)):(0,i.Q3)("",!0),(0,i.Lk)("a",{href:"javascript:;",onClick:t[0]||(t[0]=(...e)=>d.handleEdit&&d.handleEdit(...e)),title:"此客户端的设备名",class:(0,s.C4)({green:d.item.Connected})},[(0,i.Lk)("span",r,(0,s.v_)(d.item.MachineName),1)],2),d.item.isSelf?((0,i.uX)(),(0,i.CE)("strong",o,[(0,i.eW)("("),(0,i.bF)(f,{size:"16"},{default:(0,i.k6)((()=>[(0,i.bF)(m)])),_:1}),(0,i.eW)(") ")])):(0,i.Q3)("",!0)])}var h=n(3830),p=n(8104),m=n(7477),f={props:["item","config"],emits:["edit","refresh"],components:{StarFilled:m.BQ2},setup(e,{emit:t}){const n=(0,p.W)(),s=(0,h.B)(),a=(0,i.EW)((()=>s.value.hasAccess("RenameSelf"))),l=(0,i.EW)((()=>s.value.hasAccess("RenameOther"))),c=(0,i.EW)((()=>s.value.config.Client.Id)),u=()=>{if(e.config){if(c.value===e.item.MachineId){if(!a.value)return}else if(!l.value)return;t("edit",e.item)}};return{item:(0,i.EW)((()=>e.item)),tuntap:n,handleEdit:u}}},g=n(1241);const v=(0,g.A)(f,[["render",d],["__scopeId","data-v-5e5e0a80"]]);var I=v},3460:function(e,t,n){n.d(t,{A:function(){return w}});var i=n(6768),s=n(4232);const a={class:"flex"},l={class:"flex-1"},c=["title"],u=["title"],r={key:0,class:"green gateway"},o={key:1},d={key:0},h={key:0,class:"flex yellow",title:"已禁用"},p={key:1,class:"flex yellow",title:"与其它设备填写IP、或本机局域网IP有冲突"},m={key:0,class:"delay green"},f={class:"delay yellow"};function g(e,t,n,g,v,I){const L=(0,i.g2)("Loading"),y=(0,i.g2)("el-icon"),k=(0,i.g2)("el-switch");return(0,i.uX)(),(0,i.CE)("div",null,[(0,i.Lk)("div",a,[(0,i.Lk)("div",l,[(0,i.Lk)("a",{href:"javascript:;",class:"a-line",onClick:t[0]||(t[0]=e=>g.handleTuntapIP(g.tuntap.list[g.item.MachineId])),title:"此设备的虚拟网卡IP"},[g.tuntap.list[g.item.MachineId].SetupError?((0,i.uX)(),(0,i.CE)("strong",{key:0,class:"red",title:g.tuntap.list[g.item.MachineId].SetupError},(0,s.v_)(g.tuntap.list[g.item.MachineId].IP),9,c)):g.tuntap.list[g.item.MachineId].Upgrade&&g.tuntap.list[g.item.MachineId].NatError?((0,i.uX)(),(0,i.CE)("strong",{key:1,class:"yellow",title:g.tuntap.list[g.item.MachineId].NatError},(0,s.v_)(g.tuntap.list[g.item.MachineId].IP),9,u)):((0,i.uX)(),(0,i.CE)(i.FK,{key:2},[g.tuntap.list[g.item.MachineId].running?((0,i.uX)(),(0,i.CE)("strong",r,(0,s.v_)(g.tuntap.list[g.item.MachineId].IP),1)):((0,i.uX)(),(0,i.CE)("strong",o,(0,s.v_)(g.tuntap.list[g.item.MachineId].IP),1))],64))])]),g.tuntap.list[g.item.MachineId].loading?((0,i.uX)(),(0,i.CE)("div",d,[(0,i.bF)(y,{size:"14",class:"loading"},{default:(0,i.k6)((()=>[(0,i.bF)(L)])),_:1})])):((0,i.uX)(),(0,i.Wv)(k,{key:1,modelValue:g.tuntap.list[g.item.MachineId].running,"onUpdate:modelValue":t[1]||(t[1]=e=>g.tuntap.list[g.item.MachineId].running=e),loading:g.tuntap.list[g.item.MachineId].loading,disabled:"",onClick:t[2]||(t[2]=e=>g.handleTuntap(g.tuntap.list[g.item.MachineId])),size:"small","inline-prompt":"","active-text":"😀","inactive-text":"😣"},null,8,["modelValue","loading"]))]),(0,i.Lk)("div",null,[(0,i.Lk)("div",null,[((0,i.uX)(!0),(0,i.CE)(i.FK,null,(0,i.pI)(g.tuntap.list[g.item.MachineId].Lans,((e,t)=>((0,i.uX)(),(0,i.CE)(i.FK,{key:t},[e.Disabled?((0,i.uX)(),(0,i.CE)("div",h,(0,s.v_)(e.IP)+" / "+(0,s.v_)(e.PrefixLength),1)):e.Exists?((0,i.uX)(),(0,i.CE)("div",p,(0,s.v_)(e.IP)+" / "+(0,s.v_)(e.PrefixLength),1)):((0,i.uX)(),(0,i.CE)("div",{key:2,class:(0,s.C4)(["flex",{green:g.tuntap.list[g.item.MachineId].running}]),title:"正常使用"},(0,s.v_)(e.IP)+" / "+(0,s.v_)(e.PrefixLength),3))],64)))),128))]),g.showDelay?((0,i.uX)(),(0,i.CE)(i.FK,{key:0},[g.tuntap.list[g.item.MachineId].Delay>=0&&g.tuntap.list[g.item.MachineId].Delay<=100?((0,i.uX)(),(0,i.CE)("div",m,(0,s.v_)(g.tuntap.list[g.item.MachineId].Delay)+"ms",1)):(0,i.Q3)("",!0),(0,i.Lk)("template",null,[(0,i.Lk)("div",f,(0,s.v_)(g.tuntap.list[g.item.MachineId].Delay)+"ms",1)])],64)):(0,i.Q3)("",!0)])])}var v=n(5241),I=n(1219),L=n(8104),y=n(7477),k=n(3830),C={props:["item","config"],emits:["edit","refresh"],components:{Loading:y.Rhj},setup(e,{emit:t}){const n=(0,L.W)(),s=(0,k.B)(),a=(0,i.EW)((()=>s.value.config.Client.Id)),l=(0,i.EW)((()=>s.value.hasAccess("TuntapChangeSelf"))),c=(0,i.EW)((()=>s.value.hasAccess("TuntapChangeOther"))),u=(0,i.EW)((()=>s.value.hasAccess("TuntapStatusSelf"))),r=(0,i.EW)((()=>s.value.hasAccess("TuntapStatusOther"))),o=(0,i.EW)((()=>2==(2&(s.value.config.Running.Tuntap||{Switch:0}).Switch))),d=t=>{if(!e.config)return;if(a.value===t.MachineId){if(!u.value)return}else if(!r.value)return;const n=t.running?(0,v.PR)(t.MachineId):(0,v.en)(t.MachineId);t.loading=!0,n.then((()=>{I.nk.success("操作成功！")})).catch((()=>{I.nk.error("操作失败！")}))},h=n=>{if(e.config||a.value==n.MachineId){if(a.value===n.MachineId){if(!l.value)return}else if(!c.value)return;n.device=e.item,t("edit",n)}},p=()=>{t("refresh")};return{item:(0,i.EW)((()=>e.item)),tuntap:n,showDelay:o,handleTuntap:d,handleTuntapIP:h,handleTuntapRefresh:p}}},E=n(1241);const M=(0,E.A)(C,[["render",g],["__scopeId","data-v-1960cd79"]]);var w=M}}]);