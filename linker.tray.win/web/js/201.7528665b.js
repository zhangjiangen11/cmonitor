"use strict";(self["webpackChunklinker_web"]=self["webpackChunklinker_web"]||[]).push([[201],{5241:function(e,t,n){n.d(t,{Jd:function(){return d},PR:function(){return u},Yh:function(){return o},ac:function(){return r},en:function(){return c},kl:function(){return a},vB:function(){return s},zi:function(){return l}});var i=n(4);const a=(e="0")=>(0,i.zG)("tuntapclient/connections",e),s=e=>(0,i.zG)("tuntapclient/removeconnection",e),l=(e="0")=>(0,i.zG)("tuntapclient/get",e),c=e=>(0,i.zG)("tuntapclient/run",e),u=e=>(0,i.zG)("tuntapclient/stop",e),r=e=>(0,i.zG)("tuntapclient/update",e),o=()=>(0,i.zG)("tuntapclient/refresh"),d=()=>(0,i.zG)("tuntapclient/SubscribePing")},7985:function(e,t,n){n.d(t,{r:function(){return u}});n(4114);var i=n(9299),a=n(3830),s=n(6768),l=n(144);const c=[],u=()=>{const e=(0,a.B)(),t=(0,s.EW)((()=>e.value.config.Client.Id)),n=(0,l.Kh)({timer:0,page:{Request:{Page:1,Size:+(localStorage.getItem("ps")||"10"),Name:"",Ids:[],Prop:"",Asc:!0},Count:0,List:[]},showDeviceEdit:!1,showAccessEdit:!1,deviceInfo:null}),u=()=>{(0,i.nD)(n.page.Request).then((i=>{n.page.Request=i.Request,n.page.Count=i.Count;for(let n in i.List)Object.assign(i.List[n],{showDel:t.value!=i.List[n].MachineId&&0==i.List[n].Connected,showAccess:t.value!=i.List[n].MachineId&&i.List[n].Connected,showReboot:i.List[n].Connected,isSelf:t.value==i.List[n].MachineId,showip:!1}),i.List[n].isSelf&&(e.value.self=i.List[n]);n.page.List=i.List;for(let e=0;e<n.page.List.length;e++)c.push(n.page.List[e])})).catch((e=>{}))},r=()=>{(0,i.nD)(n.page.Request).then((i=>{for(let a in i.List){const s=n.page.List.filter((e=>e.MachineId==i.List[a].MachineId))[0];s&&(Object.assign(s,{Connected:i.List[a].Connected,Version:i.List[a].Version,LastSignIn:i.List[a].LastSignIn,Args:i.List[a].Args,showDel:t.value!=i.List[a].MachineId&&0==i.List[a].Connected,showAccess:t.value!=i.List[a].MachineId&&i.List[a].Connected,showReboot:i.List[a].Connected,isSelf:t.value==i.List[a].MachineId}),s.isSelf&&(e.value.self=s))}n.timer=setTimeout(r,5e3)})).catch((e=>{n.timer=setTimeout(r,5e3)}))},o=()=>{try{if(0==c.length)return void setTimeout(o,1e3);const e=c.shift();fetch(`http://ip-api.com/json/${e.IP.split(":")[0]}`).then((async t=>{try{const n=await t.json();e.countryFlag=`https://unpkg.com/flag-icons@7.2.3/flags/4x3/${n.countryCode.toLowerCase()}.svg`}catch(n){}setTimeout(o,1e3)})).catch((()=>{setTimeout(o,1e3)}))}catch(e){setTimeout(o,1e3)}};o();const d=e=>{n.deviceInfo=e,n.showDeviceEdit=!0},h=e=>{n.deviceInfo=e,n.showAccessEdit=!0},m=e=>{e&&(n.page.Request.Page=e),u()},p=e=>{e&&(n.page.Request.Size=e,localStorage.setItem("ps",e)),u()},g=e=>{(0,i.Se)(e).then((()=>{u()}))},f=()=>{clearTimeout(n.timer),n.timer=0},v=e=>(0,i.VN)(e);return{devices:n,machineId:t,_getSignList:u,_getSignList1:r,handleDeviceEdit:d,handleAccessEdit:h,handlePageChange:m,handlePageSizeChange:p,handleDel:g,clearDevicesTimeout:f,setSort:v}}},8104:function(e,t,n){n.d(t,{O:function(){return r},W:function(){return o}});var i=n(3830),a=n(1219),s=n(144),l=n(6768),c=n(5241);const u=Symbol(),r=()=>{(0,i.B)();const e=(0,s.KR)({timer:0,showEdit:!1,current:null,list:{},hashcode:0});(0,l.Gt)(u,e);const t={linux:["debian","ubuntu","alpine","rocky","centos"],windows:["windows"],android:["android"],ios:["ios"]},n=()=>{clearTimeout(e.value.timer),(0,c.zi)(e.value.hashcode.toString()).then((i=>{if(e.value.hashcode=i.HashCode,i.List){for(let e in i.List){let n="system";const a=i.List[e].SystemInfo.toLowerCase();for(let e in t)if(a.indexOf(e)>=0){const i=t[e];if(1==i.length)n=i[0];else for(let e=0;e<i.length;e++)if(a.indexOf(i[e])>=0){n=i[e];break}break}Object.assign(i.List[e],{running:2==i.List[e].Status,loading:1==i.List[e].Status,system:n,systemDocker:a.indexOf("docker")>=0})}e.value.list=i.List}e.value.timer=setTimeout(n,1100),(0,c.Jd)()})).catch((t=>{e.value.timer=setTimeout(n,1100)}))},r=t=>{e.value.current=t,e.value.showEdit=!0},o=()=>{(0,c.Yh)(),a.nk.success({message:"刷新成功",grouping:!0})},d=()=>{clearTimeout(e.value.timer),e.value.timer=0},h=t=>Object.values(e.value.list).filter((e=>e.IP.indexOf(t)>=0||e.LanIPs.filter((e=>e.indexOf(t)>=0)).length>0)).map((e=>e.MachineId)),m=t=>{const n=Object.values(e.value.list).filter((e=>e.IP)).sort(((e,t)=>{const n=e.IP.split(".").map((e=>Number(e))),i=t.IP.split(".").map((e=>Number(e)));for(let a=0;a<n.length;a++)if(n[a]!=i[a])return n[a]-i[a];return 0}));return n.map((e=>e.MachineId))};return{tuntap:e,_getTuntapInfo:n,handleTuntapEdit:r,handleTuntapRefresh:o,clearTuntapTimeout:d,getTuntapMachines:h,sortTuntapIP:m}},o=()=>(0,l.WQ)(u)},886:function(e,t,n){n.d(t,{A:function(){return v}});var i=n(6768),a=n(4232);const s=["title"],l=["src"],c=["src"],u={key:1,class:"system",src:"/docker.svg"},r={key:1};function o(e,t,n,o,d,h){const m=(0,i.g2)("StarFilled"),p=(0,i.g2)("el-icon");return(0,i.uX)(),(0,i.CE)("div",null,[o.tuntap.list[o.item.MachineId]&&o.tuntap.list[o.item.MachineId].system?((0,i.uX)(),(0,i.CE)("span",{key:0,title:o.tuntap.list[o.item.MachineId].SystemInfo},[o.item.countryFlag?((0,i.uX)(),(0,i.CE)("img",{key:0,class:"system",src:o.item.countryFlag},null,8,l)):(0,i.Q3)("",!0),(0,i.Lk)("img",{class:"system",src:`/${o.tuntap.list[o.item.MachineId].system}.svg`},null,8,c),o.tuntap.list[o.item.MachineId].systemDocker?((0,i.uX)(),(0,i.CE)("img",u)):(0,i.Q3)("",!0)],8,s)):(0,i.Q3)("",!0),(0,i.Lk)("a",{href:"javascript:;",onClick:t[0]||(t[0]=(...e)=>o.handleEdit&&o.handleEdit(...e)),title:"此客户端的设备名",class:(0,a.C4)({green:o.item.Connected})},(0,a.v_)(o.item.MachineName),3),o.item.isSelf?((0,i.uX)(),(0,i.CE)("strong",r,[(0,i.eW)(" - ("),(0,i.bF)(p,null,{default:(0,i.k6)((()=>[(0,i.bF)(m)])),_:1}),(0,i.eW)(" 本机) ")])):(0,i.Q3)("",!0)])}var d=n(3830),h=n(8104),m=n(7477),p={props:["item","config"],emits:["edit","refresh"],components:{StarFilled:m.BQ2},setup(e,{emit:t}){const n=(0,h.W)(),a=(0,d.B)(),s=(0,i.EW)((()=>a.value.hasAccess("RenameSelf"))),l=(0,i.EW)((()=>a.value.hasAccess("RenameOther"))),c=(0,i.EW)((()=>a.value.config.Client.Id)),u=()=>{if(e.config){if(c.value===e.item.MachineId){if(!s.value)return}else if(!l.value)return;t("edit",e.item)}};return{item:(0,i.EW)((()=>e.item)),tuntap:n,handleEdit:u}}},g=n(1241);const f=(0,g.A)(p,[["render",o],["__scopeId","data-v-a998806a"]]);var v=f},3630:function(e,t,n){n.d(t,{A:function(){return C}});var i=n(6768),a=n(4232);const s={class:"flex"},l={class:"flex-1"},c=["title"],u={class:"red"},r={key:0},o={class:"yellow"},d={key:1},h={key:0,class:"delay green"},m={class:"delay yellow"};function p(e,t,n,p,g,f){const v=(0,i.g2)("el-popover"),I=(0,i.g2)("Loading"),L=(0,i.g2)("el-icon"),k=(0,i.g2)("el-switch");return(0,i.uX)(),(0,i.CE)("div",null,[(0,i.Lk)("div",s,[(0,i.Lk)("div",l,[(0,i.Lk)("a",{href:"javascript:;",class:"a-line",onClick:t[0]||(t[0]=e=>p.handleTuntapIP(p.tuntap.list[p.item.MachineId])),title:p.tuntap.list[p.item.MachineId].Gateway?"我在路由器上，所以略有不同":"此设备的虚拟网卡IP"},[p.tuntap.list[p.item.MachineId].Error?((0,i.uX)(),(0,i.Wv)(v,{key:0,placement:"top",title:"msg",width:"20rem",trigger:"hover",content:p.tuntap.list[p.item.MachineId].Error},{reference:(0,i.k6)((()=>[(0,i.Lk)("strong",u,(0,a.v_)(p.tuntap.list[p.item.MachineId].IP),1)])),_:1},8,["content"])):((0,i.uX)(),(0,i.CE)(i.FK,{key:1},[p.tuntap.list[p.item.MachineId].running?((0,i.uX)(),(0,i.CE)("strong",{key:0,class:(0,a.C4)(["green",{gateway:p.tuntap.list[p.item.MachineId].Gateway}])},(0,a.v_)(p.tuntap.list[p.item.MachineId].IP),3)):((0,i.uX)(),(0,i.CE)("strong",{key:1,class:(0,a.C4)({gateway:p.tuntap.list[p.item.MachineId].Gateway})},(0,a.v_)(p.tuntap.list[p.item.MachineId].IP),3))],64))],8,c)]),p.tuntap.list[p.item.MachineId].loading?((0,i.uX)(),(0,i.CE)("div",r,[(0,i.bF)(L,{size:"14",class:"loading"},{default:(0,i.k6)((()=>[(0,i.bF)(I)])),_:1})])):((0,i.uX)(),(0,i.Wv)(k,{key:1,modelValue:p.tuntap.list[p.item.MachineId].running,"onUpdate:modelValue":t[1]||(t[1]=e=>p.tuntap.list[p.item.MachineId].running=e),loading:p.tuntap.list[p.item.MachineId].loading,disabled:"",onClick:t[2]||(t[2]=e=>p.handleTuntap(p.tuntap.list[p.item.MachineId])),size:"small","inline-prompt":"","active-text":"😀","inactive-text":"😣"},null,8,["modelValue","loading"]))]),(0,i.Lk)("div",null,[p.tuntap.list[p.item.MachineId].Error1?((0,i.uX)(),(0,i.Wv)(v,{key:0,placement:"top",title:"msg",width:"20rem",trigger:"hover",content:p.tuntap.list[p.item.MachineId].Error1},{reference:(0,i.k6)((()=>[(0,i.Lk)("div",o,[((0,i.uX)(!0),(0,i.CE)(i.FK,null,(0,i.pI)(p.tuntap.list[p.item.MachineId].LanIPs,((e,t)=>((0,i.uX)(),(0,i.CE)("div",{key:t},(0,a.v_)(e)+" / "+(0,a.v_)(p.tuntap.list[p.item.MachineId].Masks[t]),1)))),128))])])),_:1},8,["content"])):((0,i.uX)(),(0,i.CE)("div",d,[((0,i.uX)(!0),(0,i.CE)(i.FK,null,(0,i.pI)(p.tuntap.list[p.item.MachineId].LanIPs,((e,t)=>((0,i.uX)(),(0,i.CE)("div",{key:t},(0,a.v_)(e)+" / "+(0,a.v_)(p.tuntap.list[p.item.MachineId].Masks[t]),1)))),128))])),p.showDelay?((0,i.uX)(),(0,i.CE)(i.FK,{key:2},[p.tuntap.list[p.item.MachineId].Delay>=0&&p.tuntap.list[p.item.MachineId].Delay<=100?((0,i.uX)(),(0,i.CE)("div",h,(0,a.v_)(p.tuntap.list[p.item.MachineId].Delay)+"ms",1)):(0,i.Q3)("",!0),(0,i.Lk)("template",null,[(0,i.Lk)("div",m,(0,a.v_)(p.tuntap.list[p.item.MachineId].Delay)+"ms",1)])],64)):(0,i.Q3)("",!0)])])}var g=n(5241),f=n(1219),v=n(8104),I=n(7477),L=n(3830),k={props:["item","config"],emits:["edit","refresh"],components:{Loading:I.Rhj},setup(e,{emit:t}){const n=(0,v.W)(),a=(0,L.B)(),s=(0,i.EW)((()=>a.value.config.Client.Id)),l=(0,i.EW)((()=>a.value.hasAccess("TuntapChangeSelf"))),c=(0,i.EW)((()=>a.value.hasAccess("TuntapChangeOther"))),u=(0,i.EW)((()=>a.value.hasAccess("TuntapStatusSelf"))),r=(0,i.EW)((()=>a.value.hasAccess("TuntapStatusOther"))),o=(0,i.EW)((()=>2==(2&(a.value.config.Running.Tuntap||{Switch:0}).Switch))),d=t=>{if(!e.config)return;if(s.value===t.MachineId){if(!u.value)return}else if(!r.value)return;const n=t.running?(0,g.PR)(t.MachineId):(0,g.en)(t.MachineId);t.loading=!0,n.then((()=>{f.nk.success("操作成功！")})).catch((()=>{f.nk.error("操作失败！")}))},h=n=>{if(e.config||s.value==n.MachineId){if(s.value===n.MachineId){if(!l.value)return}else if(!c.value)return;n.device=e.item,t("edit",n)}},m=()=>{t("refresh")};return{item:(0,i.EW)((()=>e.item)),tuntap:n,showDelay:o,handleTuntap:d,handleTuntapIP:h,handleTuntapRefresh:m}}},y=n(1241);const M=(0,y.A)(k,[["render",p],["__scopeId","data-v-8a7ccef2"]]);var C=M}}]);