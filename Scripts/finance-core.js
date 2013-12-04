$(function(){
	window.App = {
		Views 		: {},
		Models		: {},
		Collections : {},
		Events	 	: {},
		Templates 	: {},
		Routes		: {}
	};

	$.fn.serializeObject = function () {
	    var o = {};
	    var a = this.serializeArray();
	    $.each(a, function () {
	        if (o[this.name] !== undefined) {
	            if (!o[this.name].push) {
	                o[this.name] = [o[this.name]];
	            }
	            o[this.name].push(this.value || '');
	        } else {
	            o[this.name] = this.value || '';
	        }
	    });
	    return o;
	};

	App.Events.Page = _.extend({}, Backbone.Events);

	App.Routes.FinanceRoute = Backbone.Router.extend({
		routes : {
			'page/:number' : 'go_to_page'
		},
		go_to_page : function(number) {
			App.Events.Page.trigger("page:change", number);
		}
	});

	App.Templates.FinanceTemaplate = [
		"<td><%=_Date%></td>",
		"<td><%=_Open%></td>",
		"<td><%=_High%></td>",
		"<td><%=_Low%></td>",
		"<td><%=_Close%></td>",
		"<td><%=_Volume%></td>"
	].join("");

	App.Templates.PageTemplate = [
		"<a class=\"page_prev glyphicon glyphicon-arrow-left\" href=\"javascript:void(0);\"></a>",
		"<%if(page_current != 0){%><span style=\"margin:5px;\"><strong><%=page_current%></strong></span><%}%>",
		"<a class=\"page_next glyphicon glyphicon-arrow-right\" href=\"javascript:void(0);\"></a>",
	].join("");

	for(var template in App.Templates) {
		if (App.Templates.hasOwnProperty(template)) {
			App.Templates[template] = _.template(App.Templates[template]);
		}
	}

	App.Models.Model = Backbone.Model.extend({
	    defaults: {
			_Date 	: null,
			_Open	: null,
			_High 	: null,
			_Low 	: null,
			_Close	: null,
			_Volume : null,
		}
	});

	App.Models.PageModel = Backbone.Model.extend({
		defaults : {
			page_next		: 1,
			page_prev		: 1,
			page_current    : 1,
            max_page        : 10000
		}
	});

	App.Collections.Collection = Backbone.Collection.extend({
		model : App.Models.Model,
		go_to_page : function(perPage, page){
			page = page - 1;
			var collection = this;
			collection = _(collection.rest(perPage * page));
			collection = _(collection.first(perPage));
			return collection.map(function(model){ return model; });
		},
		url : 'Finance/FinanceList'
	});

	App.Views.PageView = Backbone.View.extend({
		el : "#finance-pagination",
		initialize : function() {
			this.model.on("change", this.render, this);
			this.render();
		},
		render: function () {
			this.$el.html(App.Templates.PageTemplate(this.model.toJSON()));
			return this;
		},
		events : {
			"click .page_prev" : "change_prev",
			"click .page_next" : "change_next"
		},
		change_prev: function (vent) {
		    var start_prev = this.model.get("page_prev");
		    if (start_prev >= 1 && window.location.hash != "#page/1") {
				var next = this.model.get("page_next");
				var prev = this.model.get("page_prev");
				var current = this.model.get("page_current");
				this.model.set({ page_next : prev, page_prev : prev - 1, page_current : current - 1 });
				window.location.hash = "page/" + prev;
			}
		},
		change_next: function (vent) {
			var next = this.model.get("page_next");
			var current = this.model.get("page_current");
			var max_page = this.model.get("max_page");
			if (next < max_page) {
			    next += 1;
			    this.model.set({ page_next: next, page_prev: next - 1, page_current: current + 1 });
			    window.location.hash = "page/" + next;
			} else {
			    $(".page_next").css({ opacity : "0.7" });
			}

		}
	});

	App.Views.SingleView = Backbone.View.extend({
		tagName : "tr",
		initialize : function() {
			this.render();
		},
		render : function() {
			this.$el.html(App.Templates.FinanceTemaplate(this.model.toJSON()));
			return this;
		}
	});

	App.Views.CollectionView = Backbone.View.extend({
	    el: "#finance-list",
	    model: new App.Models.PageModel(),
		initialize : function() {
			this.collection.on("add", this.add_one, this);
			this.collection.on("reset", this.reset, this);
			App.Events.Page.on("page:change", this.render, this);
			new App.Views.PageView({ model: this.model });
		},
		add_one : function(model) {
			var _sview = new App.Views.SingleView({ model : model });
			this.$el.append(_sview.render().el);
		},
		render: function (page) {
			this.$el.empty();
			var page_number = (page) ? page : 0;
			var length = this.collection.size();
			var that = this;
			if ((page_number != 0) && (length <= (page_number - 1) * 10)) 
			{
			    $("#progress").css({ display: "block" });
			    var do_fake = $("form[name=form-data]").serializeObject();
			    var _temp_collection = new App.Collections.Collection();

			    _temp_collection.fetch({
				    data: {
				        Service     : do_fake["Service"],
				        PlainText   : do_fake["PlainText"],
				        Quote       : do_fake["Quote"],
				        DateFrom    : do_fake["DateFrom"],
				        DateTo      : do_fake["DateTo"],				        
				        Format      : do_fake["Format"],
                        page        : page_number
				    },
                    type : "GET",
                    success: function (finance_list) {
                        $("#progress").css({ display: "none" });
                        that.$el.empty();
                        that.collection.add(finance_list.models);
                        if ((that.collection.size() / 10) < page_number)
                        {
                            that.model.set({ max_page: page_number });
                        }
                        else
                        {
                            that.model.set({ max_page: 100000 });
                        }
					}
			    });

			    _temp_collection = null;
			} 
			else 
			{
				_.each(this.collection.go_to_page(10, page_number), this.add_one, this);
			}

			return this;
		},
		reset: function () {
		    
		    this.model.set({ page_current: 1, page_next: 1, page_prev: 1, max_page: 10000 });
		    this.render(1);
		}
	});

	$(document).ready(function () {
	    App.Collections.DataCollection = new App.Collections.Collection();
	    App.Views.DataView = new App.Views.CollectionView({ collection : App.Collections.DataCollection });
	});
}());